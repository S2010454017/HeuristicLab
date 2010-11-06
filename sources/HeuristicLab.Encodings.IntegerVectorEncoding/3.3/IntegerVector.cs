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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableClass]
  [Item("IntegerVector", "Represents a vector of integer values.")]
  public class IntegerVector : IntArray {
    [StorableConstructor]
    protected IntegerVector(bool deserializing) : base(deserializing) { }
    protected IntegerVector(IntegerVector original, Cloner cloner) : base(original, cloner) { }
    public IntegerVector() : base() { }
    public IntegerVector(int length) : base(length) { }
    public IntegerVector(int length, IRandom random, int min, int max)
      : this(length) {
      Randomize(random, min, max);
    }
    public IntegerVector(int[] elements) : base(elements) { }
    public IntegerVector(IntArray elements)
      : this(elements.Length) {
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i];
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerVector(this, cloner);
    }

    public virtual void Randomize(IRandom random, int startIndex, int length, int min, int max) {
      if (length > 0) {
        for (int i = 0; i < length; i++)
          array[startIndex + i] = random.Next(min, max);
        OnReset();
      }
    }
    public void Randomize(IRandom random, int min, int max) {
      Randomize(random, 0, Length, min, max);
    }
  }
}
