#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicDataAnalysisAlleleFrequencyAnalyzer", "")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisAlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<ISymbolicExpressionTree>, ISymbolicDataAnalysisAnalyzer {
    private const string AlleleTreeDepthParameterName = "AlleleTreeDepth";

    #region parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return SolutionParameter; }
    }

    public ILookupParameter<Optimization.ResultCollection> ResultCollectionParameter {
      get { return ResultsParameter; }
    }

    public IValueParameter<IntValue> AlleleTreeDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[AlleleTreeDepthParameterName]; }
    }
    #endregion
    #region properties
    public int AlleleTreeDepth {
      get { return AlleleTreeDepthParameter.Value.Value; }
      set { AlleleTreeDepthParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisAlleleFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisAlleleFrequencyAnalyzer(SymbolicDataAnalysisAlleleFrequencyAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisAlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(AlleleTreeDepthParameterName, "The depth of subtrees that should be considered as allele", new IntValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisAlleleFrequencyAnalyzer(this, cloner);
    }

    protected override Allele[] CalculateAlleles(ISymbolicExpressionTree solution) {
      List<Allele> alleles = new List<Allele>();

      foreach (var subtree in GetAllSubtreesOfDepth(solution, AlleleTreeDepth)) {
        alleles.Add(GetAlleleFromSubtreeOfDepth(subtree, AlleleTreeDepth));
      }
      return alleles.ToArray();
    }

    private Allele GetAlleleFromSubtreeOfDepth(ISymbolicExpressionTreeNode tree, int d) {
      string textualRepresentation = GetTextualRepresentationFromSubtreeOfDepth(tree, d);
      return new Allele(textualRepresentation);
    }

    private string GetTextualRepresentationFromSubtreeOfDepth(ISymbolicExpressionTreeNode tree, int d) {
      if (d == 0) return "";
      StringBuilder builder = new StringBuilder();
      builder.Append("(" + tree.ToString());
      for (int i = 0; i < tree.SubtreesCount; i++) {
        builder.Append(" " + GetTextualRepresentationFromSubtreeOfDepth(tree.GetSubtree(i), d - 1));
      }
      builder.Append(")");
      return builder.ToString();
    }

    private IEnumerable<ISymbolicExpressionTreeNode> GetAllSubtreesOfDepth(ISymbolicExpressionTree solution, int d) {
      return from node in solution.IterateNodesPostfix()
             where node.GetDepth() >= d
             select node;
    }
  }
}
