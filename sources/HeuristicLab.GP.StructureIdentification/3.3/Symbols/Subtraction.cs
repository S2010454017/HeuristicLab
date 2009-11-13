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


namespace HeuristicLab.GP.StructureIdentification {
  public sealed class Subtraction : FunctionBase {
    public override string Description {
      get {
        return @"Subtracts the results of sub-tree 2..n from the result of the first sub-tree.
    (- 3) => -3
    (- 2 3) => -1
    (- 3 4 5) => -6";
      }
    }

    public Subtraction()
      : base() {
      MinSubTrees = 1;
      MaxSubTrees = 3;
    }

    public override HeuristicLab.GP.Interfaces.IFunctionTree GetTreeNode() {
      return new FunctionTreeBase(this);
    }
  }
}
