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
using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("e16ad337-8c18-4c29-a893-e83f671e804c")]
  /// <summary>
  /// Interface to represent an optimization problem.
  /// </summary>
  public interface IProblem : IParameterizedNamedItem {
    IEnumerable<IItem> Operators { get; }

    IEnumerable<IParameterizedItem> ExecutionContextItems { get; }
    event EventHandler OperatorsChanged;
    event EventHandler Reset;
  }

  [StorableType("1b4af8b9-bdf5-4ffd-86e6-35b481bfbf45")]
  public interface IProblem<TEncoding, TEncodedSolution> : IHeuristicOptimizationProblem
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution { }
}