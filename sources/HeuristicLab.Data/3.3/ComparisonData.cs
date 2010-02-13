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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [EmptyStorableClass]
  [Item("ComparisonData", "Represents a comparison.")]
  [Creatable("Test")]
  public sealed class ComparisonData : ValueTypeData<Comparison>, IComparable {
    public ComparisonData() : base() { }
    public ComparisonData(Comparison value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      ComparisonData clone = new ComparisonData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public int CompareTo(object obj) {
      ComparisonData other = obj as ComparisonData;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }
  }
}
