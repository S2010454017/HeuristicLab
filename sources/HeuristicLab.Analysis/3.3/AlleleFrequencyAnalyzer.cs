#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator for analyzing the frequency of alleles.
  /// </summary>
  [Item("AlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles.")]
  [StorableClass]
  public abstract class AlleleFrequencyAnalyzer<T> : SingleSuccessorOperator, IAnalyzer where T : class, IItem {
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<T> SolutionParameter {
      get { return (ScopeTreeLookupParameter<T>)Parameters["Solution"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<T> BestKnownSolutionParameter {
      get { return (LookupParameter<T>)Parameters["BestKnownSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public ValueParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters["UpdateInterval"]; }
    }
    public LookupParameter<IntValue> UpdateCounterParameter {
      get { return (LookupParameter<IntValue>)Parameters["UpdateCounter"]; }
    }

    [StorableConstructor]
    protected AlleleFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    public AlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<T>("Solution", "The solutions whose alleles should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<T>("BestKnownSolution", "The best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the allele frequency analysis results should be stored."));
      Parameters.Add(new ValueParameter<BoolValue>("StoreHistory", "True if the history of the allele frequency analysis should be stored.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("UpdateInterval", "The interval in which the allele frequency analysis should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>("UpdateCounter", "The value which counts how many times the operator was called since the last update.", "AlleleFrequencyAnalyzerUpdateCounter"));
    }

    #region AlleleFrequencyIdEqualityComparer
    private class AlleleFrequencyIdEqualityComparer : IEqualityComparer<AlleleFrequency> {
      public bool Equals(AlleleFrequency x, AlleleFrequency y) {
        return x.Id == y.Id;
      }
      public int GetHashCode(AlleleFrequency obj) {
        return obj.Id.GetHashCode();
      }
    }
    #endregion

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;
      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      } else updateCounter.Value++;

      if (updateCounter.Value == updateInterval) {
        updateCounter.Value = 0;

        bool max = MaximizationParameter.ActualValue.Value;
        ItemArray<T> solutions = SolutionParameter.ActualValue;
        ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
        T bestKnownSolution = BestKnownSolutionParameter.ActualValue;
        bool storeHistory = StoreHistoryParameter.Value.Value;

        // calculate index of current best solution
        int bestIndex = -1;
        if (!max) bestIndex = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
        else bestIndex = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

        // calculate allels of current best and (if available) best known solution
        Allele[] bestAlleles = CalculateAlleles(solutions[bestIndex]);
        Allele[] bestKnownAlleles = null;
        if (bestKnownSolution != null)
          bestKnownAlleles = CalculateAlleles(bestKnownSolution);

        // calculate allele frequencies
        var frequencies = solutions.SelectMany((s, index) => CalculateAlleles(s).Select(a => new { Allele = a, Quality = qualities[index] })).
                          GroupBy(x => x.Allele.Id).
                          Select(x => new AlleleFrequency(x.Key,
                                                          x.Count() / ((double)solutions.Length),
                                                          x.Average(a => a.Allele.Impact),
                                                          x.Average(a => a.Quality.Value),
                                                          bestKnownAlleles == null ? false : bestKnownAlleles.Any(a => a.Id == x.Key),
                                                          bestAlleles.Any(a => a.Id == x.Key)));

        // calculate dummy allele frequencies of alleles of best known solution which did not occur
        if (bestKnownAlleles != null) {
          var bestKnownFrequencies = bestKnownAlleles.Select(x => new AlleleFrequency(x.Id, 0, x.Impact, 0, true, false)).Except(frequencies, new AlleleFrequencyIdEqualityComparer());
          frequencies = frequencies.Concat(bestKnownFrequencies);
        }

        // fetch results collection
        ResultCollection results;
        if (!ResultsParameter.ActualValue.ContainsKey("Allele Frequency Analysis Results")) {
          results = new ResultCollection();
          ResultsParameter.ActualValue.Add(new Result("Allele Frequency Analysis Results", results));
        } else {
          results = (ResultCollection)ResultsParameter.ActualValue["Allele Frequency Analysis Results"].Value;
        }

        // store allele frequencies
        AlleleFrequencyCollection frequenciesCollection = new AlleleFrequencyCollection(frequencies);
        if (!results.ContainsKey("Allele Frequencies"))
          results.Add(new Result("Allele Frequencies", frequenciesCollection));
        else
          results["Allele Frequencies"].Value = frequenciesCollection;

        // store allele frequencies history
        if (storeHistory) {
          if (!results.ContainsKey("Allele Frequencies History")) {
            AlleleFrequencyCollectionHistory history = new AlleleFrequencyCollectionHistory();
            history.Add(frequenciesCollection);
            results.Add(new Result("Allele Frequencies History", history));
          } else {
            ((AlleleFrequencyCollectionHistory)results["Allele Frequencies History"].Value).Add(frequenciesCollection);
          }
        }

        // store alleles data table
        DataTable allelesTable;
        if (!results.ContainsKey("Alleles")) {
          allelesTable = new DataTable("Alleles");
          results.Add(new Result("Alleles", allelesTable));
          allelesTable.Rows.Add(new DataRow("Unique Alleles"));
          DataRowVisualProperties visualProperties = new DataRowVisualProperties();
          visualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Line;
          visualProperties.SecondYAxis = true;
          visualProperties.StartIndexZero = true;
          allelesTable.Rows.Add(new DataRow("Unique Alleles of Best Known Solution", null, visualProperties));
          allelesTable.Rows.Add(new DataRow("Fixed Alleles", null, visualProperties));
          allelesTable.Rows.Add(new DataRow("Fixed Alleles of Best Known Solution", null, visualProperties));
          allelesTable.Rows.Add(new DataRow("Lost Alleles of Best Known Solution", null, visualProperties));
        } else {
          allelesTable = (DataTable)results["Alleles"].Value;
        }

        int fixedAllelesCount = frequenciesCollection.Where(x => x.Frequency == 1).Count();
        var relevantAlleles = frequenciesCollection.Where(x => x.ContainedInBestKnownSolution);
        int relevantAllelesCount = relevantAlleles.Count();
        int fixedRelevantAllelesCount = relevantAlleles.Where(x => x.Frequency == 1).Count();
        int lostRelevantAllelesCount = relevantAlleles.Where(x => x.Frequency == 0).Count();
        int uniqueRelevantAllelesCount = relevantAllelesCount - lostRelevantAllelesCount;
        allelesTable.Rows["Unique Alleles"].Values.Add(frequenciesCollection.Count);
        allelesTable.Rows["Unique Alleles of Best Known Solution"].Values.Add(uniqueRelevantAllelesCount);
        allelesTable.Rows["Fixed Alleles"].Values.Add(fixedAllelesCount);
        allelesTable.Rows["Fixed Alleles of Best Known Solution"].Values.Add(fixedRelevantAllelesCount);
        allelesTable.Rows["Lost Alleles of Best Known Solution"].Values.Add(lostRelevantAllelesCount);
      }
      return base.Apply();
    }

    protected abstract Allele[] CalculateAlleles(T solution);
  }
}
