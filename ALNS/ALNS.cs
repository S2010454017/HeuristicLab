using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ALNS.AcceptanceMethods;
using ALNS.DestroyHeuristics;
using ALNS.RepairHeuristics;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Random;

namespace ALNS {
  [Item("Adaptive Large Neigbhourhood Search (ALNS)", "An implementation of the Adaptive Large Neigbhourhood Search algorithm.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 999)]
  [StorableType("3ebf7f68-9122-44e4-baef-c6aa9115c543")]
  public class ALNS : BasicAlgorithm {
    public override bool SupportsPause => true;

    #region ProblemProperties
    public override Type ProblemType { get { return typeof(VehicleRoutingProblem); } }
    public new VehicleRoutingProblem Problem { get { return (VehicleRoutingProblem)base.Problem; } }
    #endregion

    #region Storable fields
    [Storable]
    private IRandom random = new MersenneTwister();
    [Storable]
    private double[] destroyWeights;
    [Storable]
    private double[] repairWeights;
    [Storable]
    private IList<ILNSDestroy> destroyHeuristics;
    [Storable]
    private IList<ILNSRepair> repairHeuristics;
    #endregion

    #region Weight Change Coefficents
    //c1 > c2 > c3 > c4 >= 0.0
    [Storable]
    private readonly double bestSolutionFactor = 6;                 //c1
    [Storable]
    private readonly double betterThanCurrentSolutionFactor = 3.0;    //c2
    [Storable]
    private readonly double solutionAcceptedFactor = 1.5;             //c3
    [Storable]
    private readonly double solutionNotAcceptedFactor = .6;          //c4       
    #endregion

    #region ParameterNames
    private const string SeedName = "Seed";
    private const string SetSeedRandomlyName = "SetSeedRandomly";
    private const string MaximumIterationsName = "MaxIterations";
    private const string HistoryInfluenceName = "HistoryInfluence";
    private const string AcceptMethodName = "AcceptMethod";
    private const string DestroyGradeName = "DestroyGrade";

    private const string IterationResultName = "Iterations";
    private const string BestQualityName = "BestQuality";
    private const string DifferenceToKnownBestName = "DiffToBest";
    private const string BestFoundSolutionGUIName = "BestFoundSolutionGUI";
    private const string AcceptedQualityName = "AcceptedQuality";
    private const string DifferenceQualityName = "DifferenceQuality";

    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> MaxIterationsParameter {
      get {
        return (IFixedValueParameter<IntValue>)Parameters[MaximumIterationsName];
      }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedName]; }
    }
    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyName]; }
    }

    public IFixedValueParameter<DoubleValue> HistoryInfluenceParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[HistoryInfluenceName]; }
    }

    public IFixedValueParameter<DoubleValue> DestroyGradeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DestroyGradeName]; }
    }

    public IConstrainedValueParameter<ILNSAccept> AcceptMethodParameter {
      get { return (IConstrainedValueParameter<ILNSAccept>)Parameters[AcceptMethodName]; }
    }

    #endregion

    #region Properties
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public int MaxIterations {
      get { return MaxIterationsParameter.Value.Value; }
      set { MaxIterationsParameter.Value.Value = value; }
    }
    public double HistoryInfluence {
      get { return HistoryInfluenceParameter.Value.Value; }
      set { HistoryInfluenceParameter.Value.Value = value; }
    }
    public double DestroyGrade {
      get { return DestroyGradeParameter.Value.Value; }
      set { DestroyGradeParameter.Value.Value = value; }
    }
    public ILNSAccept AcceptMethod {
      get { return AcceptMethodParameter.Value; }
      set { AcceptMethodParameter.Value = value; }
    }
    #endregion

    #region ResultProperties
    private VRPSolution BestFoundSolutionGUIResult {
      set { Results[BestFoundSolutionGUIName].Value = value; }
    }
    private int IterationResult {
      get { return ((IntValue)Results[IterationResultName].Value).Value; }
      set { ((IntValue)Results[IterationResultName].Value).Value = value; }
    }
    private double BestQualityResult {
      get { return ((DoubleValue)Results[BestQualityName].Value).Value; }
      set { ((DoubleValue)Results[BestQualityName].Value).Value = value; }
    }
    private double DifferenceToKnownBestResult {
      get { return ((DoubleValue)Results[DifferenceToKnownBestName].Value).Value; }
      set { ((DoubleValue)Results[DifferenceToKnownBestName].Value).Value = value; }
    }

    private DataTable QualitiesResults {
      get { return (DataTable)Results[AcceptedQualityName].Value; }
    }

    private DataRow AcceptedQualityResult {
      get { return QualitiesResults.Rows[AcceptedQualityName]; }
    }

    private DataRow DifferenceQualityResult {
      get { return QualitiesResults.Rows[DifferenceQualityName]; }
    }


    #endregion

    #region Constructors
    [StorableConstructor]
    protected ALNS(StorableConstructorFlag _) : base(_) { }
    public ALNS(ALNS original, Cloner cloner) : base(original, cloner) {
      random = cloner.Clone(original.random);
      destroyWeights = (double[])original.destroyWeights?.Clone();
      repairWeights = (double[])original.repairWeights?.Clone();
      destroyHeuristics = original.destroyHeuristics.ToList();
      repairHeuristics = original.repairHeuristics.ToList();
      bestSolutionFactor = original.bestSolutionFactor;
      betterThanCurrentSolutionFactor = original.betterThanCurrentSolutionFactor;
      solutionAcceptedFactor = original.solutionAcceptedFactor;
      solutionNotAcceptedFactor = original.solutionNotAcceptedFactor;
    }
    public ALNS() {
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumIterationsName, "The maximum number of iteration to perform", new IntValue(1_000)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(HistoryInfluenceName, "0 if history should not influence new values, 1 if only history matters.", new DoubleValue(.5)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(DestroyGradeName, "Determine how big the destruction should be (in percentage)", new DoubleValue(.3)));
      var setAccept = new ItemSet<ILNSAccept> {
        new SimulatedAnnelingAcceptance(),
        new RandomWalkAcceptance(),
        new GreedyAcceptance() };
      Parameters.Add(new ConstrainedValueParameter<ILNSAccept>(AcceptMethodName, "Strategy how new solutions should be accepted", setAccept, setAccept.First()));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ALNS(this, cloner);
    }
    #endregion

    #region Initialization
    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
      if (SetSeedRandomly) {
        Seed = RandomSeedGenerator.GetSeed();
      }
      random.Reset(Seed);

      destroyHeuristics = new List<ILNSDestroy> {
        new RandomRemovel(random, DestroyGrade, Problem.ProblemInstance),
        new WorstRemovel(DestroyGrade, Problem.ProblemInstance)
      };
     
      repairHeuristics = new List<ILNSRepair> {
        new GreedyRepair(),
        new RegretRepair()
      };

      destroyWeights = Enumerable.Repeat(1.0, destroyHeuristics.Count()).ToArray();
      repairWeights = Enumerable.Repeat(1.0, repairHeuristics.Count()).ToArray();

      InitReults();
    }

    private void InitReults() {
      Results.Add(new Result(IterationResultName, "Number of executed iterations", new IntValue(0)));
      Results.Add(new Result(BestQualityName, "Quality of best found solution", new DoubleValue(double.MaxValue)));
      Results.Add(new Result(DifferenceToKnownBestName, "Difference to known best solution", new DoubleValue(double.NaN)));
      Results.Add(new Result(BestFoundSolutionGUIName, typeof(VRPSolution)));

      foreach (var destroy in destroyHeuristics) {
        Results.Add(new Result(destroy.ItemName, $"Iteration in which {destroy.ItemName} was used", new ItemList<IntValue>()));
      }

      foreach (var repair in repairHeuristics) {
        Results.Add(new Result(repair.ItemName, $"Iteration in which {repair.ItemName} was used", new ItemList<IntValue>()));
      }

      var table = new DataTable("QualityIndicators");
      table.Rows.Add(new DataRow(AcceptedQualityName));
      table.Rows.Add(new DataRow(DifferenceQualityName));
      Results.Add(new Result(AcceptedQualityName, "Quality of accepted solutions in a timeline", table));
    }
    #endregion

    protected override void Run(CancellationToken cancellationToken) {
      int indexDestroy = 0;                   //index of selected destroy method
      int indexRepair = 0;                    //index of selected repair method
      double lambda = 1 - HistoryInfluence;   //needed for weight change
      double weightChangeFactor = .0;         //the factor for changeing the weight of the selected methods
      double bestKnowQualityFromProblem = Problem.BestKnownQuality?.Value != null ?
                                          Problem.BestKnownQuality.Value : double.NaN;

      //create random init solution
      PotvinEncoding solution = PushForwardInsertionCreator.CreateSolution(Problem.ProblemInstance, this.random);
      BestFoundSolutionGUIResult = new VRPSolution(Problem.ProblemInstance,
                                                   new PotvinEncoding(Problem.ProblemInstance),
                                                   new DoubleValue(double.MaxValue));

      //holds the last accepted solution
      PotvinEncoding lastAcceptedSolution = (PotvinEncoding)solution.Clone();
      var evalLastAccepted = Problem.ProblemInstance.Evaluate(lastAcceptedSolution);
      try {
        while ((IterationResult < MaxIterations) && (bestKnowQualityFromProblem.CompareTo(BestQualityResult) != 0)) {

          //calculate probabilities for each heuristic
          var propabilityDestroy = destroyWeights.Select(v => v / destroyWeights.Sum()).ToList();
          var propabilityRepair = repairWeights.Select(v => v / repairWeights.Sum()).ToList();

          double nextDestroy = random.NextDouble();
          double nextRepair = random.NextDouble();

          //select first destroy/repair method whose probability is nearest to the generated randomnumber
          indexDestroy = propabilityDestroy.Select((p, i) => new { index = i, diff = Math.Abs(p - nextDestroy) })
                                           .OrderBy(p => p.diff)
                                           .First()
                                           .index;

          indexRepair = propabilityRepair.Select((p, i) => new { index = i, diff = Math.Abs(p - nextRepair) })
                                         .OrderBy(p => p.diff)
                                         .First()
                                         .index;

          // document which method was choosen each iteration
          ((ItemList<IntValue>)Results[destroyHeuristics[indexDestroy].ItemName].Value).Add(new IntValue(IterationResult));
          ((ItemList<IntValue>)Results[repairHeuristics[indexRepair].ItemName].Value).Add(new IntValue(IterationResult));


          //create new solution with repair(destroy(x))
          solution = destroyHeuristics[indexDestroy].DestroySolution(lastAcceptedSolution);
          solution = repairHeuristics[indexRepair].RepairSolution(solution, Problem.ProblemInstance);


          //check if new solution is accepted (always, worse sometimes, simulated anneling)
          if (AcceptMethod.AcceptSolution(lastAcceptedSolution, solution, Problem.ProblemInstance)) {
            weightChangeFactor = solutionAcceptedFactor;
            var evalutionNewSolution = Problem.ProblemInstance.Evaluate(solution);


            //check if new solution is better than s_best
            if (evalutionNewSolution.Quality.CompareTo(BestQualityResult) < 0) {
              BestFoundSolutionGUIResult = new VRPSolution(Problem.ProblemInstance,
                                                           solution,
                                                           new DoubleValue(evalutionNewSolution.Quality));

              BestQualityResult = evalutionNewSolution.Quality;
              DifferenceToKnownBestResult = bestKnowQualityFromProblem == double.NaN ? double.NaN :
                                            Math.Abs(bestKnowQualityFromProblem - BestQualityResult);
              weightChangeFactor = bestSolutionFactor;
            } else {
              //check if at least improving current solution
              if (evalutionNewSolution.Quality.CompareTo(evalLastAccepted.Quality) < 0) {
                weightChangeFactor = betterThanCurrentSolutionFactor;
              }
            }

            //document solution quality
            AcceptedQualityResult.Values.Add(evalutionNewSolution.Quality);
            DifferenceQualityResult.Values.Add(DifferenceToKnownBestResult);

            //update current solution
            lastAcceptedSolution = (PotvinEncoding)solution.Clone();
          } else {
            weightChangeFactor = solutionNotAcceptedFactor;
          }

          //update weight of destroy- and repairmethod
          destroyWeights[indexDestroy] = destroyWeights[indexDestroy] * HistoryInfluence + (lambda * weightChangeFactor);
          repairWeights[indexRepair] = repairWeights[indexRepair] * HistoryInfluence + (lambda * weightChangeFactor);


          ++IterationResult;
          cancellationToken.ThrowIfCancellationRequested();
        }
      } 
      catch(OperationCanceledException) {
        //nothing to do, user stopped algorithm
      }
      catch (Exception) {
        throw;
      }
    }
  }
}
