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
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis {
  //TODO Change to new empty problem interface
  [StorableType("74e4c570-3881-4bfa-a5bb-2bb71cdee2b3")]
  public interface IDataAnalysisProblem : IProblem {
    IDataAnalysisProblemData ProblemData { get; }
    event EventHandler ProblemDataChanged;
  }

  [StorableType("c2f6fcdd-ab62-4423-be75-01aa694df411")]
  public interface IDataAnalysisProblem<T> : IDataAnalysisProblem
  where T : class, IDataAnalysisProblemData {
    IValueParameter<T> ProblemDataParameter { get; }
    new T ProblemData { get; set; }
  }
}
