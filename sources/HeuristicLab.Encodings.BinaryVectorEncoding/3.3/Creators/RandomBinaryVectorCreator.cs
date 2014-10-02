﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Generates a new random binary vector with each element randomly initialized.
  /// </summary>
  [Item("RandomBinaryVectorCreator", "An operator which creates a new random binary vector with each element randomly initialized.")]
  [StorableClass]
  public sealed class RandomBinaryVectorCreator : BinaryVectorCreator {
    private const string TrueProbabilityParameterName = "TruePropability";

    private IFixedValueParameter<DoubleValue> TrueProbabilityParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[TrueProbabilityParameterName]; }
    }

    public double TrueProbability {
      get { return TrueProbabilityParameter.Value.Value; }
      set { TrueProbabilityParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private RandomBinaryVectorCreator(bool deserializing) : base(deserializing) { }

    private RandomBinaryVectorCreator(RandomBinaryVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new RandomBinaryVectorCreator(this, cloner); }

    public RandomBinaryVectorCreator()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(TrueProbabilityParameterName, "Probability of true value", new DoubleValue(0.5)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(TrueProbabilityParameterName))
        Parameters.Add(new FixedValueParameter<DoubleValue>(TrueProbabilityParameterName, "Probability of true value", new DoubleValue(0.5)));
    }

    /// <summary>
    /// Generates a new random binary vector with the given <paramref name="length"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="length">The length of the binary vector.</param>
    /// <param name="trueProbability">The propability for true to occur at a certain position in the binary vector</param>
    /// <returns>The newly created binary vector.</returns>
    public static BinaryVector Apply(IRandom random, int length, double trueProbability = 0.5) {
      BinaryVector result;

      //Backwards compatiblity code to ensure the same behavior for existing algorithm runs
      //remove with HL 3.4
      if (trueProbability.IsAlmost(0.5))
      result = new BinaryVector(length, random);
      else {
        var values = new bool[length];
        for (int i = 0; i < length; i++)
          values[i] = random.NextDouble() < trueProbability;
        result = new BinaryVector(values);
      }
      return result;
    }

    protected override BinaryVector Create(IRandom random, IntValue length) {
      return Apply(random, length.Value, TrueProbability);
    }
  }
}
