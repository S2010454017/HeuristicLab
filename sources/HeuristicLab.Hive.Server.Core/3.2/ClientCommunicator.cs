﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Core;
using HeuristicLab.Hive.Server.DataAccess;
using System.Resources;
using System.Reflection;
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using System.Threading;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// The ClientCommunicator manages the whole communication with the client
  /// </summary>
  public class ClientCommunicator: IClientCommunicator {
    private static Dictionary<Guid, DateTime> lastHeartbeats = 
      new Dictionary<Guid,DateTime>();

    private static ReaderWriterLockSlim heartbeatLock =
      new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    private ISessionFactory factory;
    private ILifecycleManager lifecycleManager;
    private IInternalJobManager jobManager;
    private IScheduler scheduler;

    /// <summary>
    /// Initialization of the Adapters to the database
    /// Initialization of Eventhandler for the lifecycle management
    /// Initialization of lastHearbeats Dictionary
    /// </summary>
    public ClientCommunicator() {
      factory = ServiceLocator.GetSessionFactory();
      
      lifecycleManager = ServiceLocator.GetLifecycleManager();
      jobManager = ServiceLocator.GetJobManager() as 
        IInternalJobManager;
      scheduler = ServiceLocator.GetScheduler();

      lifecycleManager.RegisterHeartbeat( 
        new EventHandler(lifecycleManager_OnServerHeartbeat));
    }

    /// <summary>
    /// Check if online clients send their hearbeats
    /// if not -> set them offline and check if they where calculating a job
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void lifecycleManager_OnServerHeartbeat(object sender, EventArgs e) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();

        tx = session.BeginTransaction();

        List<ClientInfo> allClients = new List<ClientInfo>(clientAdapter.GetAll());

        foreach (ClientInfo client in allClients) {
          if (client.State != State.offline && client.State != State.nullState) {
            heartbeatLock.EnterUpgradeableReadLock();

            if (!lastHeartbeats.ContainsKey(client.Id)) {
              client.State = State.offline;
              clientAdapter.Update(client);
              foreach (Job job in jobAdapter.GetActiveJobsOf(client)) {
                jobManager.ResetJobsDependingOnResults(job);
              }
            } else {
              DateTime lastHbOfClient = lastHeartbeats[client.Id];

              TimeSpan dif = DateTime.Now.Subtract(lastHbOfClient);
              // check if time between last hearbeat and now is greather than HEARTBEAT_MAX_DIF
              if (dif.TotalSeconds > ApplicationConstants.HEARTBEAT_MAX_DIF) {
                // if client calculated jobs, the job must be reset
                foreach (Job job in jobAdapter.GetActiveJobsOf(client)) {
                  jobManager.ResetJobsDependingOnResults(job);
                }

                // client must be set offline
                client.State = State.offline;
                clientAdapter.Update(client);

                heartbeatLock.EnterWriteLock();
                lastHeartbeats.Remove(client.Id);
                heartbeatLock.ExitWriteLock();
              }
            }

            heartbeatLock.ExitUpgradeableReadLock();
          } else {
            heartbeatLock.EnterWriteLock();
            if (lastHeartbeats.ContainsKey(client.Id))
              lastHeartbeats.Remove(client.Id);
            heartbeatLock.ExitWriteLock();
          }
        }
        tx.Commit();
      }
      catch (Exception ex) {
        if (tx != null) 
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    #region IClientCommunicator Members

    /// <summary>
    /// Login process for the client
    /// A hearbeat entry is created as well (login is the first hearbeat)
    /// </summary>
    /// <param name="clientInfo"></param>
    /// <returns></returns>
    public Response Login(ClientInfo clientInfo) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

        tx = session.BeginTransaction();

        Response response = new Response();

        heartbeatLock.EnterWriteLock();
        if (lastHeartbeats.ContainsKey(clientInfo.Id)) {
          lastHeartbeats[clientInfo.Id] = DateTime.Now;
        } else {
          lastHeartbeats.Add(clientInfo.Id, DateTime.Now);
        }
        heartbeatLock.ExitWriteLock();

        ClientInfo client = clientAdapter.GetById(clientInfo.Id);
        if (client != null && client.State != State.offline && client.State != State.nullState) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE;
          return response;
        }
        clientInfo.State = State.idle;
        clientAdapter.Update(clientInfo);
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_SUCCESS;

        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// The client has to send regulary heartbeats
    /// this hearbeats will be stored in the heartbeats dictionary
    /// check if there is work for the client and send the client a response if he should pull a job
    /// </summary>
    /// <param name="hbData"></param>
    /// <returns></returns>
    public ResponseHB ProcessHeartBeat(HeartBeatData hbData) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();

        tx = session.BeginTransaction();

        ResponseHB response = new ResponseHB();
        response.ActionRequest = new List<MessageContainer>();

        ClientInfo client = clientAdapter.GetById(hbData.ClientId);

        // check if the client is logged in
        if (client.State == State.offline || client.State == State.nullState) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_USER_NOT_LOGGED_IN;
          response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));
          return response;
        }

        client.NrOfFreeCores = hbData.FreeCores;
        client.FreeMemory = hbData.FreeMemory;

        // save timestamp of this heartbeat
        heartbeatLock.EnterWriteLock();
        if (lastHeartbeats.ContainsKey(hbData.ClientId)) {
          lastHeartbeats[hbData.ClientId] = DateTime.Now;
        } else {
          lastHeartbeats.Add(hbData.ClientId, DateTime.Now);
        }
        heartbeatLock.ExitWriteLock();

        // check if client has a free core for a new job
        // if true, ask scheduler for a new job for this client
        if (hbData.FreeCores > 0 && scheduler.ExistsJobForClient(hbData))
          response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.FetchJob));
        else
          response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_HEARTBEAT_RECEIVED;

        processJobProcess(hbData, jobAdapter, clientAdapter, response);
        clientAdapter.Update(client);

        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Process the Job progress sent by a client
    /// </summary>
    /// <param name="hbData"></param>
    /// <param name="jobAdapter"></param>
    /// <param name="clientAdapter"></param>
    /// <param name="response"></param>
    private void processJobProcess(HeartBeatData hbData, IJobAdapter jobAdapter, IClientAdapter clientAdapter, ResponseHB response) {
      if (hbData.JobProgress != null) {
        List<Job> jobsOfClient = new List<Job>(jobAdapter.GetActiveJobsOf(clientAdapter.GetById(hbData.ClientId)));
        if (jobsOfClient == null || jobsOfClient.Count == 0) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
          return;
        }

        foreach (KeyValuePair<Guid, double> jobProgress in hbData.JobProgress) {
          Job curJob = jobAdapter.GetById(jobProgress.Key);
          if (curJob.Client == null || curJob.Client.Id != hbData.ClientId) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
          } else if (curJob.State == State.abort) {
            // a request to abort the job has been set
            response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.AbortJob, curJob.Id));
            curJob.State = State.offline;
          } else {
            // save job progress
            curJob.Percentage = jobProgress.Value;
            jobAdapter.Update(curJob);

            if (curJob.State == State.requestSnapshot) {
              // a request for a snapshot has been set
              response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.RequestSnapshot, curJob.Id));
              curJob.State = State.calculating;
            }
          }
        }
      }
    }
   
    /// <summary>
    /// if the client was told to pull a job he calls this method
    /// the server selects a job and sends it to the client
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public ResponseJob SendJob(Guid clientId) {
      ResponseJob response = new ResponseJob();

      Job job2Calculate = scheduler.GetNextJobForClient(clientId);
      if (job2Calculate != null) {
        response.Job = job2Calculate;
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_PULLED;
      } else {
        response.Success = false;
        response.Job = null;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT;
      }
      return response;
    }

    private ResponseResultReceived ProcessJobResult(Guid clientId,
      Guid jobId,
      byte[] result,
      double percentage,
      Exception exception,
      bool finished) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();
        IJobResultsAdapter jobResultAdapter =
          session.GetDataAdapter<JobResult, IJobResultsAdapter>();

        tx = session.BeginTransaction();

        ResponseResultReceived response = new ResponseResultReceived();
        ClientInfo client =
          clientAdapter.GetById(clientId);

        Job job =
          jobAdapter.GetById(jobId);

        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOB_WITH_THIS_ID;
          response.JobId = jobId;
          return response;
        }
        if (job.Client == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
          response.JobId = jobId;
          return response;
        }
        if (job.Client.Id != clientId) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_CLIENT_FOR_JOB;
          response.JobId = jobId;
          return response;
        }
        if (job.State == State.finished) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
          response.JobId = jobId;
          return response;
        }
        if (job.State != State.calculating) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_JOB_STATE;
          response.JobId = jobId;
          return response;
        }
        job.SerializedJob = result;
        job.Percentage = percentage;

        if (finished) {
          job.State = State.finished;
          jobAdapter.Update(job);

          client.State = State.idle;
          clientAdapter.Update(client);

          List<JobResult> jobResults = new List<JobResult>(jobResultAdapter.GetResultsOf(job));
          foreach (JobResult currentResult in jobResults)
            jobResultAdapter.Delete(currentResult);
        }

        JobResult jobResult =
          new JobResult();
        jobResult.Client = client;
        jobResult.Job = job;
        jobResult.Result = result;
        jobResult.Percentage = percentage;
        jobResult.Exception = exception;
        jobResult.DateFinished = DateTime.Now;

        jobResultAdapter.Update(jobResult);
        jobAdapter.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
        response.JobId = jobId;
        response.finished = finished;

        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }


    /// <summary>
    /// the client can send job results during calculating 
    /// and will send a final job result when he finished calculating
    /// these job results will be stored in the database
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="jobId"></param>
    /// <param name="result"></param>
    /// <param name="exception"></param>
    /// <param name="finished"></param>
    /// <returns></returns>
    public ResponseResultReceived StoreFinishedJobResult(Guid clientId, 
      Guid jobId, 
      byte[] result, 
      double percentage,
      Exception exception) {

      return ProcessJobResult(clientId, jobId, result, percentage, exception, true);
    }


    public ResponseResultReceived ProcessSnapshot(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      return ProcessJobResult(clientId, jobId, result, percentage, exception, false);
    }

    /// <summary>
    /// when a client logs out the state will be set
    /// and the entry in the last hearbeats dictionary will be removed
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>                       
    public Response Logout(Guid clientId) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();

        tx = session.BeginTransaction();

        Response response = new Response();

        heartbeatLock.EnterWriteLock();
        if (lastHeartbeats.ContainsKey(clientId))
          lastHeartbeats.Remove(clientId);
        heartbeatLock.ExitWriteLock();

        ClientInfo client = clientAdapter.GetById(clientId);
        if (client == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED;
          return response;
        }
        List<Job> allJobs = new List<Job>(jobAdapter.GetAll());
        if (client.State == State.calculating) {
          // check wich job the client was calculating and reset it
          foreach (Job job in allJobs) {
            if (job.Client.Id == client.Id) {
              jobManager.ResetJobsDependingOnResults(job);
            }
          }
        }

        client.State = State.offline;
        clientAdapter.Update(client);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS;

        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// If a client goes offline and restores a job he was calculating 
    /// he can ask the client if he still needs the job result
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Response IsJobStillNeeded(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();
        tx = session.BeginTransaction();

        Response response = new Response();
        Job job = jobAdapter.GetById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_DOESNT_EXIST;
          return response;
        }
        if (job.State == State.finished) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_ALLREADY_FINISHED;
          return response;
        }
        job.State = State.finished;
        jobAdapter.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_SEND_JOBRESULT;
        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public ResponsePlugin SendPlugins(List<HivePluginInfo> pluginList) {


      throw new NotImplementedException();
    }

    #endregion
  }
}
