﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Parameter-less Population Pyramid (P3)", "Binary value optimization algorithm which requires no configuration. B. W. Goldman and W. F. Punch, Parameter-less Population Pyramid, GECCO, pp. 785–792, 2014")]
  [StorableType("CAD84CAB-1ECC-4D76-BDC5-701AAF690E17")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 400)]
  public class ParameterlessPopulationPyramid : BasicAlgorithm {
    public override Type ProblemType {
      get { return typeof(SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>); }
    }
    // TODO: Type of this property should be ISingleObjectiveProblemDefinition instead of the SingleObjectiveProblem
    //       However, this requires that BasicAlgorithm's Problem property is also changed
    public new SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector> Problem {
      get { return (SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>)base.Problem; }
      set { base.Problem = value; }
    }

    [Storable]
    private readonly IRandom random = new MersenneTwister();
    [Storable]
    private List<Population> pyramid = new List<Population>();
    [Storable]
    private EvaluationTracker tracker;

    // Tracks all solutions in Pyramid for quick membership checks

    private HashSet<BinaryVector> seen = new HashSet<BinaryVector>(new EnumerableBoolEqualityComparer());
    [Storable]
    private IEnumerable<BinaryVector> StorableSeen {
      get { return seen; }
      set { seen = new HashSet<BinaryVector>(value, new EnumerableBoolEqualityComparer()); }
    }

    #region ParameterNames
    private const string MaximumIterationsParameterName = "Maximum Iterations";
    private const string MaximumEvaluationsParameterName = "Maximum Evaluations";
    private const string MaximumRuntimeParameterName = "Maximum Runtime";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumIterationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumEvaluationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumEvaluationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumRuntimeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumRuntimeParameterName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    #endregion

    #region Properties
    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }
    public int MaximumEvaluations {
      get { return MaximumEvaluationsParameter.Value.Value; }
      set { MaximumEvaluationsParameter.Value.Value = value; }
    }
    public int MaximumRuntime {
      get { return MaximumRuntimeParameter.Value.Value; }
      set { MaximumRuntimeParameter.Value.Value = value; }
    }
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    #endregion

    #region ResultsProperties
    [Storable] private DoubleValue resultsBestQuality;
    public double ResultsBestQuality {
      get => resultsBestQuality.Value;
      set => resultsBestQuality.Value = value;
    }
    [Storable] private IntValue resultsBestFoundOnEvaluation;
    public int ResultsBestFoundOnEvaluation {
      get => resultsBestFoundOnEvaluation.Value;
      set => resultsBestFoundOnEvaluation.Value = value;
    }
    [Storable] private IntValue resultsEvaluations;
    public int ResultsEvaluations {
      get => resultsEvaluations.Value;
      set => resultsEvaluations.Value = value;
    }
    [Storable] private IntValue resultsIterations;
    public int ResultsIterations {
      get => resultsIterations.Value;
      set => resultsIterations.Value = value;
    }
    [Storable] public DataRow ResultsQualitiesBest { get; private set; }
    [Storable] public DataRow ResultsQualitiesIteration { get; private set; }
    [Storable] public DataRow ResultsLevels { get; private set; }
    [Storable] public DataRow ResultsSolutions { get; private set; }
    #endregion

    public override bool SupportsPause { get { return true; } }

    [StorableConstructor]
    protected ParameterlessPopulationPyramid(StorableConstructorFlag _) : base(_) { }

    protected ParameterlessPopulationPyramid(ParameterlessPopulationPyramid original, Cloner cloner)
      : base(original, cloner) {
      random = cloner.Clone(original.random);
      pyramid = original.pyramid.Select(cloner.Clone).ToList();
      tracker = cloner.Clone(original.tracker);
      seen = new HashSet<BinaryVector>(original.seen.Select(cloner.Clone), new EnumerableBoolEqualityComparer());
      resultsBestQuality = cloner.Clone(original.resultsBestQuality);
      resultsBestFoundOnEvaluation = cloner.Clone(original.resultsBestFoundOnEvaluation);
      resultsEvaluations = cloner.Clone(original.resultsEvaluations);
      resultsIterations = cloner.Clone(original.resultsIterations);
      ResultsQualitiesBest = cloner.Clone(original.ResultsQualitiesBest);
      ResultsQualitiesIteration = cloner.Clone(original.ResultsQualitiesIteration);
      ResultsLevels = cloner.Clone(original.ResultsLevels);
      ResultsSolutions = cloner.Clone(original.ResultsSolutions);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParameterlessPopulationPyramid(this, cloner);
    }

    public ParameterlessPopulationPyramid() : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumIterationsParameterName, "", new IntValue(Int32.MaxValue)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumEvaluationsParameterName, "", new IntValue(Int32.MaxValue)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumRuntimeParameterName, "The maximum runtime in seconds after which the algorithm stops. Use -1 to specify no limit for the runtime", new IntValue(3600)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
    }

    protected override void OnExecutionTimeChanged() {
      base.OnExecutionTimeChanged();
      if (CancellationTokenSource == null) return;
      if (MaximumRuntime == -1) return;
      if (ExecutionTime.TotalSeconds > MaximumRuntime) CancellationTokenSource.Cancel();
    }

    private void AddIfUnique(BinaryVector solution, int level) {
      // Don't add things you have seen
      if (seen.Contains(solution)) return;
      if (level == pyramid.Count) {
        pyramid.Add(new Population(Problem.Encoding.Length, random));
      }
      var copied = (BinaryVector)solution.Clone();
      pyramid[level].Add(copied);
      seen.Add(copied);
    }

    // In the GECCO paper, Figure 1
    private double iterate() {
      // Create a random solution
      BinaryVector solution = new BinaryVector(Problem.Encoding.Length);
      for (int i = 0; i < solution.Length; i++) {
        solution[i] = random.Next(2) == 1;
      }
      double fitness = tracker.Evaluate(solution, random).Quality;
      fitness = HillClimber.ImproveToLocalOptimum(tracker, solution, fitness, random);
      AddIfUnique(solution, 0);

      for (int level = 0; level < pyramid.Count; level++) {
        var current = pyramid[level];
        double newFitness = LinkageCrossover.ImproveUsingTree(current.Tree, current.Solutions, solution, fitness, tracker, random);
        // add it to the next level if its a strict fitness improvement
        if (tracker.IsBetter(newFitness, fitness)) {
          fitness = newFitness;
          AddIfUnique(solution, level + 1);
        }
      }
      return fitness;
    }

    protected override void Initialize(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = RandomSeedGenerator.GetSeed();
      pyramid = new List<Population>();
      seen.Clear();
      random.Reset(Seed);                                                          
      tracker = new EvaluationTracker(Problem, MaximumEvaluations);

      // Set up the results display
      if (!Results.TryGetValue("Iterations", out var result))
        Results.Add(new Result("Iterations", "The current iteration.", resultsIterations = new IntValue(0)));
      else result.Value = resultsIterations = new IntValue(0);
      if (!Results.TryGetValue("Evaluations", out var result2))
        Results.Add(new Result("Evaluations", "The number of evaluations that have been performed.", resultsEvaluations = new IntValue(0)));
      else result2.Value = resultsEvaluations = new IntValue(0);
      if (!Results.TryGetValue("Best Quality", out var result4))
        Results.Add(new Result("Best Quality", "The best quality that has been observed so far.", resultsBestQuality = new DoubleValue(tracker.BestQuality)));
      else result4.Value = resultsBestQuality = new DoubleValue(tracker.BestQuality);
      if (!Results.TryGetValue("Evaluation Best Solution Was Found", out var result5))
        Results.Add(new Result("Evaluation Best Solution Was Found", "The number of evaluations at which the best solution was found.", resultsBestFoundOnEvaluation = new IntValue(tracker.BestFoundOnEvaluation)));
      else result5.Value = resultsBestFoundOnEvaluation = new IntValue(tracker.BestFoundOnEvaluation);
      var table = new DataTable("Qualities");
      table.Rows.Add(ResultsQualitiesBest = new DataRow("Best Quality"));
      table.Rows.Add(ResultsQualitiesIteration = new DataRow("Iteration Quality"));
      ResultsQualitiesIteration.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Dot;
      if (!Results.TryGetValue("Qualities", out var result6))
        Results.Add(new Result("Qualities", "An analysis of the quality progress over time.", table));
      else result6.Value = table;

      table = new DataTable("Pyramid Levels");
      table.Rows.Add(ResultsLevels = new DataRow("Levels"));
      if (!Results.TryGetValue("Pyramid Levels", out var result7))
        Results.Add(new Result("Pyramid Levels", "The number of levels of the pyramid.", table));
      else result7.Value = table;

      table = new DataTable("Stored Solutions");
      table.Rows.Add(ResultsSolutions = new DataRow("Solutions"));
      if (!Results.TryGetValue("Stored Solutions", out var result8))
        Results.Add(new Result("Stored Solutions", "The number of solutions that are found over time.", table));
      else result8.Value = table;

      base.Initialize(cancellationToken);
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Loop until iteration limit reached or canceled.
      while (ResultsIterations < MaximumIterations) {
        double fitness = double.NaN;

        try {
          fitness = iterate();
          ResultsIterations++;
          cancellationToken.ThrowIfCancellationRequested();
        }
        finally {
          ResultsEvaluations = tracker.Evaluations;
          ResultsBestQuality = tracker.BestQuality;
          ResultsBestFoundOnEvaluation = tracker.BestFoundOnEvaluation;
          ResultsQualitiesBest.Values.Add(tracker.BestQuality);
          ResultsQualitiesIteration.Values.Add(fitness);
          ResultsLevels.Values.Add(pyramid.Count);
          ResultsSolutions.Values.Add(seen.Count);
        }
      }
    }
  }
}
