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

using System;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.TestFunctions.SingleObjective.Views {
  /// <summary>
  /// Class for viewing the single objective test functions problem.
  /// </summary>
  [View("Single Objective Test Function View")]
  [Content(typeof(SingleObjectiveTestFunction), true)]
  public sealed partial class SingleObjectiveTestFunctionView : ProblemView {
    public new SingleObjectiveTestFunction Content {
      get { return (SingleObjectiveTestFunction)base.Content; }
      set { base.Content = value; }
    }

    public SingleObjectiveTestFunctionView() {
      InitializeComponent();
    }

    public SingleObjectiveTestFunctionView(SingleObjectiveTestFunction content)
      : this() {
      Content = content;
    }
  }
}
