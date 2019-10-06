﻿using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.Instances.QAPLIB;
using HeuristicLab.Problems.QuadraticAssignment;
using HeuristicLab.Random;

public class GAQAPScript : HeuristicLab.Scripting.CSharpScriptBase {
  public override void Main() {
    DateTime start = DateTime.UtcNow;

    QuadraticAssignmentProblem qap;
    if (vars.Contains("qap")) qap = vars.qap;
    else {
      var provider = new DreznerQAPInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name == "dre56");
      var data = provider.LoadData(instance);
      qap = new QuadraticAssignmentProblem();
      qap.Load(data);
      vars.qap = qap;
    }

    const uint seed = 0;
    const int popSize = 100;
    const int generations = 1000;
    const double mutationRate = 0.05;

    var random = new MersenneTwister(seed);
    var population = new Permutation[popSize];
    var qualities = new double[popSize];
    var nextGen = new Permutation[popSize];
    var nextQual = new double[popSize];

    var qualityChart = new DataTable("Quality Chart");
    var qualityRow = new DataRow("Best Quality");
    qualityChart.Rows.Add(qualityRow);
    vars.qualityChart = qualityChart;

    for (int i = 0; i < popSize; i++) {
      population[i] = new Permutation(PermutationTypes.Absolute, qap.Weights.Rows, random);
      qualities[i] = qap.Evaluate(population[i], CancellationToken.None);
    }
    var bestQuality = qualities.Min();
    var bestQualityGeneration = 0;

    for (int g = 0; g < generations; g++) {
      var parents = population.SampleProportional(random, 2 * popSize, qualities, windowing: true, inverseProportional: true).ToArray();
      for (int i = 0; i < popSize; i++) {
        nextGen[i] = PartiallyMatchedCrossover.Apply(random, parents[i * 2], parents[i * 2 + 1]);
        if (random.NextDouble() < mutationRate) Swap2Manipulator.Apply(random, nextGen[i]);
        nextQual[i] = qap.Evaluate(nextGen[i], CancellationToken.None);
        if (nextQual[i] < bestQuality) {
          bestQuality = nextQual[i];
          bestQualityGeneration = g;
        }
      }
      qualityRow.Values.Add(bestQuality);
      Array.Copy(nextGen, population, popSize);
      Array.Copy(nextQual, qualities, popSize);
    }

    vars.elapsed = new TimeSpanValue(DateTime.UtcNow - start);
    vars.bestQuality = bestQuality;
    vars.bestQualityFoundAt = bestQualityGeneration;
  }
}