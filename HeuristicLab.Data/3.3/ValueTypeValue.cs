#region License Information
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.JsonInterface;

namespace HeuristicLab.Data {
  [Item("ValueTypeValue", "An abstract base class for representing values of value types.")]
  [StorableType("A78FF29D-A796-463F-A93F-2528A382D99E")]
  public abstract class ValueTypeValue<T> : Item, IJsonConvertable where T : struct {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.ValueType; }
    }

    [Storable]
    protected T value;
    public virtual T Value {
      get { return value; }
      set {
        if (ReadOnly) throw new NotSupportedException("Value cannot be set. ValueTypeValue is read-only.");
        if (!value.Equals(this.value)) {
          this.value = value;
          OnValueChanged();
        }
      }
    }

    [Storable]
    protected bool readOnly;
    public virtual bool ReadOnly {
      get { return readOnly; }
    }

    [StorableConstructor]
    protected ValueTypeValue(StorableConstructorFlag _) : base(_) { }
    protected ValueTypeValue(ValueTypeValue<T> original, Cloner cloner)
      : base(original, cloner) {
      this.value = original.value;
      this.readOnly = original.readOnly;
    }
    protected ValueTypeValue() {
      this.value = default(T);
      this.readOnly = false;
    }
    protected ValueTypeValue(T value) {
      this.value = value;
      this.readOnly = false;
    }

    public virtual ValueTypeValue<T> AsReadOnly() {
      ValueTypeValue<T> readOnlyValueTypeValue = (ValueTypeValue<T>)this.Clone();
      readOnlyValueTypeValue.readOnly = true;
      return readOnlyValueTypeValue;
    }

    public override string ToString() {
      return value.ToString();
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
    }

    public void Inject(JsonItem data, JsonItemConverter converter) {
      Value = data.GetProperty<T>(nameof(Value));
    }

    public JsonItem Extract(JsonItemConverter converter) {
      var item = new EmptyJsonItem(ItemName, this, converter) {
        Name = ItemName,
        Description = ItemDescription
      };
      item.AddProperty<T>(nameof(Value), Value);
      return item;
    }
  }
}
