﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;
using System.Threading;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  class DefaultScheduler: IScheduler {

    //private ISessionFactory factory;

    private static Mutex jobLock =
      new Mutex();

    #region IScheduler Members

    public DefaultScheduler() {
      //factory = ServiceLocator.GetSessionFactory();
    }

    public bool ExistsJobForClient(HeuristicLab.Hive.Contracts.BusinessObjects.HeartBeatData hbData) {
      //ISession session = factory.GetSessionForCurrentThread();

      //try {
        /*IJobAdapter jobAdapter =
          session.GetDataAdapter<JobDto, IJobAdapter>();*/

        List<JobDto> allOfflineJobsForClient = new List<JobDto>(DaoLocator.JobDao.FindFittingJobsForClient(State.offline, hbData.FreeCores, hbData.FreeMemory));
          /*jobAdapter.FindJobs(State.offline, 
          hbData.FreeCores,
          hbData.FreeMemory, 
          hbData.ClientId));*/
        return (allOfflineJobsForClient != null && allOfflineJobsForClient.Count > 0);
      //}
      /*finally {
        if (session != null)
          session.EndSession();
      } */
    }   

    public HeuristicLab.Hive.Contracts.BusinessObjects.JobDto GetNextJobForClient(Guid clientId) {
      /*ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<JobDto, IJobAdapter>();

        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientDto, IClientAdapter>();*/

        /// Critical section ///
        jobLock.WaitOne();

        ClientDto client = DaoLocator.ClientDao.FindById(clientId);
      LinkedList<JobDto> allOfflineJobsForClient =
        new LinkedList<JobDto>(DaoLocator.JobDao.FindFittingJobsForClient(State.offline, client.NrOfFreeCores,
                                                                          client.FreeMemory));

        JobDto jobToCalculate = null;
        if (allOfflineJobsForClient != null && allOfflineJobsForClient.Count > 0) {
          jobToCalculate = allOfflineJobsForClient.First.Value;
          jobToCalculate.State = State.calculating;
          jobToCalculate.Client = client;
          jobToCalculate.Client.State = State.calculating;          
          jobToCalculate.DateCalculated = DateTime.Now;
          DaoLocator.JobDao.AssignClientToJob(client.Id, jobToCalculate.Id);
          DaoLocator.JobDao.Update(jobToCalculate);
          DaoLocator.ClientDao.Update(jobToCalculate.Client);
        }
        jobLock.ReleaseMutex();
        /// End Critical section ///

        return jobToCalculate;
      }
      /*finally {
        if (session != null)
          session.EndSession();
      } 
    }   */

    #endregion
  }
}
