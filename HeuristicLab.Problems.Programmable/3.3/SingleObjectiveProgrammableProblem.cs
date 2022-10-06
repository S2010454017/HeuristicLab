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

using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [Item("Programmable Problem (single-objective)", "Represents a single-objective problem that can be programmed with a script.")]
  [StorableType("44944E6B-E95E-4805-8F0A-0C0F7D761DB9")]
  public abstract class SingleObjectiveProgrammableProblem<TEncoding, TEncodedSolution> : SingleObjectiveProblem<TEncoding, TEncodedSolution>, IProgrammableItem, IProgrammableProblem
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {
    protected static readonly string ENCODING_NAMESPACE = "ENCODING_NAMESPACE";
    protected static readonly string ENCODING_CLASS = "ENCODING_CLASS";
    protected static readonly string SOLUTION_CLASS = "SOLUTION_CLASS";

    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    private FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>> SingleObjectiveProblemScriptParameter {
      get { return (FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>)Parameters["ProblemScript"]; }
    }

    Script IProgrammableProblem.ProblemScript {
      get { return ProblemScript; }
    }
    public SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> ProblemScript {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    public ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> ProblemDefinition {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    protected SingleObjectiveProgrammableProblem(SingleObjectiveProgrammableProblem<TEncoding, TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    [StorableConstructor]
    protected SingleObjectiveProgrammableProblem(StorableConstructorFlag _) : base(_) { }
    public SingleObjectiveProgrammableProblem(TEncoding encoding)
      : base(encoding) {
      Parameters.Add(new FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>("ProblemScript", "Defines the problem.", new SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>() { Name = Name }));
      ProblemScript.Encoding = (TEncoding)encoding.Clone();

      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, typeof(TEncoding).Namespace);
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, typeof(TEncoding).Name);
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, typeof(TEncodedSolution).Name);
      ProblemScript.Code = codeTemplate;

      Operators.Add(new BestScopeSolutionAnalyzer());
      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      ProblemScript.ProblemDefinitionChanged += (o, e) => OnProblemDefinitionChanged();
      ProblemScript.NameChanged += (o, e) => OnProblemScriptNameChanged();
    }

    private void OnProblemDefinitionChanged() {
      Maximization = ProblemDefinition.Maximization;
      Encoding = (TEncoding)ProblemScript.Encoding.Clone();

      OnOperatorsChanged();
      OnReset();
    }
    protected override void OnNameChanged() {
      base.OnNameChanged();
      ProblemScript.Name = Name;
    }
    private void OnProblemScriptNameChanged() {
      Name = ProblemScript.Name;
    }

    public override void Evaluate(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random, CancellationToken cancellationToken) {
      ProblemDefinition.Evaluate(solutionContext, random, cancellationToken);
    }
    public override ISingleObjectiveEvaluationResult Evaluate(TEncodedSolution individual, IRandom random, CancellationToken cancellationToken) {
      return ProblemDefinition.Evaluate(individual, random, cancellationToken);
    }

    public override void Analyze(ISingleObjectiveSolutionContext<TEncodedSolution>[] solutionContexts, IRandom random) {
      ProblemDefinition.Analyze(solutionContexts, random);
    }

    public override IEnumerable<ISingleObjectiveSolutionContext<TEncodedSolution>> GetNeighbors(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random) {
      return ProblemDefinition.GetNeighbors(solutionContext, random);
    }
    public override IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution individual, IRandom random) {
      return ProblemDefinition.GetNeighbors(individual, random);
    }
  }
}
