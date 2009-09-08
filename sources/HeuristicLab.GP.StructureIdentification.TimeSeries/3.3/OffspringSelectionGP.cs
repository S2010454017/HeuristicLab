﻿#region License Information
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

using HeuristicLab.Core;
using HeuristicLab.Modeling;
using HeuristicLab.Operators;
using System;
using HeuristicLab.Data;
using HeuristicLab.GP.Algorithms;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class OffspringSelectionGP : HeuristicLab.GP.StructureIdentification.OffspringSelectionGP, ITimeSeriesAlgorithm {

    public int MinTimeOffset {
      get { return GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data = value; }
    }

    public int MaxTimeOffset {
      get { return GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data = value; }
    }

    public bool UseEstimatedTargetValue {
      get { return GetVariableInjector().GetVariable("UseEstimatedTargetValue").GetValue<BoolData>().Data; }
      set { GetVariableInjector().GetVariable("UseEstimatedTargetValue").GetValue<BoolData>().Data = value; }
    }

    protected override IOperator CreateProblemInjector() {
      return DefaultTimeSeriesOperators.CreateProblemInjector();
    }

    protected override IOperator CreateFunctionLibraryInjector() {
      return DefaultTimeSeriesOperators.CreateFunctionLibraryInjector();
    }

    protected override IOperator CreatePostProcessingOperator() {
      return DefaultTimeSeriesOperators.CreatePostProcessingOperator();
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector injector = base.CreateGlobalInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("MinTimeOffset", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTimeOffset", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("UseEstimatedTargetValue", new BoolData()));
      return injector;
    }

    protected override IAnalyzerModel CreateGPModel() {
      IAnalyzerModel model = base.CreateGPModel();
      DefaultTimeSeriesOperators.SetModelData(model, Engine.GlobalScope.SubScopes[0]);
      return model;
    }
  }
}
