﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("EC99F3C1-D8D2-4738-9523-0D07438647A5")]
  [Item("InvertedGenerationalDistanceAnalyzer", "This analyzer is functionally equivalent to the InvertedGenerationalDistanceAnalyzer in HeuristicLab.Analysis, but is kept as not to break backwards compatibility")]
  public class InvertedGenerationalDistanceAnalyzer : MOTFAnalyzer {
    public override bool EnabledByDefault {
      get { return false; }
    }

    private IFixedValueParameter<DoubleValue> DampeningParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Dampening"]; }
    }

    public double Dampening {
      get { return DampeningParameter.Value.Value; }
      set { DampeningParameter.Value.Value = value; }
    }

    public IResultParameter<DoubleValue> InvertedGenerationalDistanceResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Inverted Generational Distance"]; }
    }

    public InvertedGenerationalDistanceAnalyzer() {
      Parameters.Add(new FixedValueParameter<DoubleValue>("Dampening", "", new DoubleValue(1)));
      Parameters.Add(new ResultParameter<DoubleValue>("Inverted Generational Distance", "The genrational distance between the current front and the optimal front"));
      InvertedGenerationalDistanceResultParameter.DefaultValue = new DoubleValue(double.NaN);
    }


    [StorableConstructor]
    protected InvertedGenerationalDistanceAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected InvertedGenerationalDistanceAnalyzer(InvertedGenerationalDistanceAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new InvertedGenerationalDistanceAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var optimalfront = TestFunctionParameter.ActualValue.OptimalParetoFront(qualities[0].Length);
      if (optimalfront == null) return base.Apply();

      var q = qualities.Select(x => x.ToArray());
      InvertedGenerationalDistanceResultParameter.ActualValue.Value = InvertedGenerationalDistance.Calculate(q, optimalfront, Dampening);
      return base.Apply();
    }
  }
}
