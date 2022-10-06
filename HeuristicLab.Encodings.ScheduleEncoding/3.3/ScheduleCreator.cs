﻿#region License Information
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduleCreator", "Represents the generalized form of creators for Scheduling Problems.")]
  [StorableType("3DDA1485-4518-4F1D-A475-795FFE63C98E")]
  public abstract class ScheduleCreator<TSchedule> : InstrumentedOperator, IScheduleCreator<TSchedule>
  where TSchedule : class, IScheduleSolution {

    public ILookupParameter<TSchedule> ScheduleParameter {
      get { return (ILookupParameter<TSchedule>)Parameters["Schedule"]; }
    }
    public IValueLookupParameter<IntValue> JobsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Jobs"]; }
    }
    public IValueLookupParameter<IntValue> ResourcesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Resources"]; }
    }

    [StorableConstructor]
    protected ScheduleCreator(StorableConstructorFlag _) : base(_) { }
    protected ScheduleCreator(ScheduleCreator<TSchedule> original, Cloner cloner) : base(original, cloner) { }
    public ScheduleCreator()
      : base() {
      Parameters.Add(new LookupParameter<TSchedule>("Schedule", "The new scheduling solution candidate."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Jobs", "The number of jobs handled in this problem instance."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Resources", "The number of resources used in this problem instance."));
    }

    public override IOperation InstrumentedApply() {
      ScheduleParameter.ActualValue = CreateSolution();
      return base.InstrumentedApply();
    }

    protected abstract TSchedule CreateSolution();
  }
}
