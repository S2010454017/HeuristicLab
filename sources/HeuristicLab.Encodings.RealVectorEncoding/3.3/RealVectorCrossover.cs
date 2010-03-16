﻿#region License Information
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of real-valued vectors.
  /// </summary>
  [Item("RealVectorCrossover", "A base class for operators that perform a crossover of real-valued vectors.")]
  [StorableClass]
  public abstract class RealVectorCrossover : SingleSuccessorOperator, IRealVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<RealVector>> ParentsParameter {
      get { return (SubScopesLookupParameter<RealVector>)Parameters["Parents"]; }
    }
    public ILookupParameter<RealVector> ChildParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Child"]; }
    }

    protected RealVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new SubScopesLookupParameter<RealVector>("Parents", "The parent vectors which should be crossed."));
      Parameters.Add(new LookupParameter<RealVector>("Child", "The child vector resulting from the crossover."));
    }

    public sealed override IOperation Apply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.Apply();
    }

    protected abstract RealVector Cross(IRandom random, ItemArray<RealVector> parents);
  }
}
