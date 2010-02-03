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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [Content(typeof(SubScopesItemParameter<>), true)]
  public partial class SubScopesItemParameterView<T> : ParameterView where T : class, IItem {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new SubScopesItemParameter<T> Content {
      get { return (SubScopesItemParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public SubScopesItemParameterView() {
      InitializeComponent();
      Caption = "SubScopesItemParameter";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public SubScopesItemParameterView(SubScopesItemParameter<T> content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ActualNameChanged -= new EventHandler(Content_ActualNameChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ActualNameChanged += new EventHandler(Content_ActualNameChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "SubScopesItemParameter";
        actualNameTextBox.Text = "-";
        actualNameTextBox.Enabled = false;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        actualNameTextBox.Text = Content.ActualName;
        actualNameTextBox.Enabled = true;
      }
    }

    protected virtual void Content_ActualNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ActualNameChanged), sender, e);
      else
        actualNameTextBox.Text = Content.ActualName;
    }

    protected virtual void actualNameTextBox_Validated(object sender, EventArgs e) {
      Content.ActualName = actualNameTextBox.Text;
    }
  }
}
