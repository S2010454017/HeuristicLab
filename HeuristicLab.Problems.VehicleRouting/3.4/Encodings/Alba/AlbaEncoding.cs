﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaEncoding", "Represents the encoding for Alba encoded solutions.")]
  [StorableType("42763c6b-358f-48a2-b134-075ed987d88c")]
  public sealed class AlbaEncoding : VRPEncoding {

    [StorableConstructor]
    private AlbaEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      DiscoverOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new AlbaEncoding(this, cloner); }
    private AlbaEncoding(AlbaEncoding original, Cloner cloner) : base(original, cloner) { }


    public AlbaEncoding() : this("VRPTours") { }
    public AlbaEncoding(string name) : base(name) {
      DiscoverOperators();
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static AlbaEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IAlbaOperator),
          typeof (IVRPCreator),
          typeof (IMultiVRPOperator),
          typeof (IMultiVRPMoveOperator)
      };
    }
    protected override void DiscoverOperators() {
      var assembly = typeof(IAlbaOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      discoveredTypes = discoveredTypes.Where(x => EncodingOperatorTypes.Except(new[] { typeof(IAlbaOperator) }).All(y => !y.IsAssignableFrom(x)));
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      foreach (var op in newOperators.OfType<IMultiVRPOperator>().ToList()) {
        op.SetOperators(Operators.Concat(newOperators));
        if (!op.Operators.Any()) newOperators.Remove(op);
      }
      foreach (var op in Operators.OfType<IMultiVRPOperator>()) {
        op.SetOperators(newOperators);
      }
      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
    }

    #region specific operator wiring

    #endregion
  }
}
