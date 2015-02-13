﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class UnwrappedBallFunctionFiveDimensional : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Vladislavleva-4 F4(X1, X2, X3, X4, X5) = 10 / (5 + Sum(Xi - 3)^2)"; } }
    public override string Description {
      get {
        return "Paper: Order of Nonlinearity as a Complexity Measure for Models Generated by Symbolic Regression via Pareto Genetic Programming " + Environment.NewLine
        + "Authors: Ekaterina J. Vladislavleva, Member, IEEE, Guido F. Smits, Member, IEEE, and Dick den Hertog" + Environment.NewLine
        + "Function: F4(X1, X2, X3, X4, X5) = 10 / (5 + Sum(Xi - 3)^2)" + Environment.NewLine
        + "Training Data: 1024 points Xi = Rand(0.05, 6.05)" + Environment.NewLine
        + "Test Data: 5000 points Xi = Rand(-0.25, 6.35)" + Environment.NewLine
        + "Function Set: +, -, *, /, square, x^eps, x + eps, x * eps";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2", "X3", "X4", "X5" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 1024; } }
    protected override int TestPartitionStart { get { return 1024; } }
    protected override int TestPartitionEnd { get { return 6024; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(1024, 0.05, 6.05).ToList());
        data[i].AddRange(ValueGenerator.GenerateUniformDistributedValues(5000, -0.25, 6.35));
      }

      double x1, x2, x3, x4, x5;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        x3 = data[2][i];
        x4 = data[3][i];
        x5 = data[4][i];
        results.Add(10 / (5 + Math.Pow(x1 - 3, 2) + Math.Pow(x2 - 3, 2) + Math.Pow(x3 - 3, 2) + Math.Pow(x4 - 3, 2) + Math.Pow(x5 - 3, 2)));
      }
      data.Add(results);

      return data;
    }
  }
}
