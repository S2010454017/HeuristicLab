#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a parameter.
  /// </summary>
  [Item("Item Parameter", "A parameter which represents an IItem.")]
  [Creatable("Test")]
  public class ItemParameter : Parameter {
    [Storable]
    private string actualName;
    public string ActualName {
      get { return actualName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (!actualName.Equals(value)) {
          actualName = value;
          OnActualNameChanged();
        }
      }
    }

    private IItem value;
    [Storable]
    public IItem Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          if ((value != null) && (!DataType.IsInstanceOfType(value))) throw new ArgumentException("Static value does not match data type of parameter");
          if (this.value != null) this.value.Changed -= new ChangedEventHandler(Value_Changed);
          this.value = value;
          if (this.value != null) this.value.Changed += new ChangedEventHandler(Value_Changed);
          OnValueChanged();
        }
      }
    }

    public ItemParameter()
      : base("Anonymous", null, typeof(IItem)) {
      actualName = Name;
      Value = null;
    }
    protected ItemParameter(string name, string description, Type dataType)
      : base(name, description, dataType) {
      this.actualName = Name;
      this.Value = null;
    }
    public ItemParameter(string name, string description)
      : base(name, description, typeof(IItem)) {
      this.actualName = Name;
      this.Value = null;
    }
    public ItemParameter(string name, string description, IItem value)
      : base(name, description, typeof(IItem)) {
      this.actualName = Name;
      this.Value = value;
    }

    public override IItem GetValue(ExecutionContext context) {
      if (Value != null) return Value;
      ExecutionContext parent = context.Parent;
      IParameter parameter = null;

      while ((parent != null) && (parameter == null)) {
        parent.Operator.Parameters.TryGetValue(ActualName, out parameter);
        parent = parent.Parent;
      }

      if (parameter != null) return parameter.GetValue(context);
      else return context.Scope.Lookup(ActualName, true).Value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ItemParameter clone = (ItemParameter)base.Clone(cloner);
      clone.actualName = actualName;
      clone.Value = (IItem)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value != null ? Value.ToString() : ActualName, DataType.Name);
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
      OnChanged();
    }
    public event EventHandler ValueChanged;
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      OnChanged();
    }

    private void Value_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }

  [Item("Parameter<T>", "A generic parameter which represents an instance of type T.")]
  [EmptyStorableClass]
  public class ItemParameter<T> : ItemParameter where T : class, IItem {
    public new T Value {
      get { return (T)base.Value; }
      set { base.Value = value; }
    }

    public ItemParameter()
      : base("Anonymous", null, typeof(T)) {
      Value = null;
    }
    public ItemParameter(string name, string description)
      : base(name, description, typeof(T)) {
      this.Value = null;
    }
    public ItemParameter(string name, string description, T value)
      : base(name, description, typeof(T)) {
      this.Value = value;
    }

    public new T GetValue(ExecutionContext context) {
      return (T)base.GetValue(context);
    }
  }
}
