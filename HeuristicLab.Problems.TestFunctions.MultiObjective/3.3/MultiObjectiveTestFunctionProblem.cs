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

using System;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("AB0C6A73-C432-46FD-AE3B-9841EAB2478C")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 95)]
  [Item("Test Function (multi-objective)", "Test functions with real valued inputs and multiple objectives.")]
  public class MultiObjectiveTestFunctionProblem : RealVectorMultiObjectiveProblem, IProblemInstanceConsumer<MOTFData>, IMultiObjectiveProblemDefinition<RealVectorEncoding, RealVector> {
    #region Parameter Properties
    public IFixedValueParameter<IntValue> ObjectivesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Objectives"]; }
    }
    public IValueParameter<IMultiObjectiveTestFunction> TestFunctionParameter {
      get { return (IValueParameter<IMultiObjectiveTestFunction>)Parameters["TestFunction"]; }
    }
    #endregion

    #region Properties
    public new int Objectives {
      get { return ObjectivesParameter.Value.Value; }
      set { ObjectivesParameter.Value.Value = value; }
    }
    public IMultiObjectiveTestFunction TestFunction {
      get { return TestFunctionParameter.Value; }
      set { TestFunctionParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiObjectiveTestFunctionProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected MultiObjectiveTestFunctionProblem(MultiObjectiveTestFunctionProblem original, Cloner cloner) : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveTestFunctionProblem(this, cloner);
    }

    public MultiObjectiveTestFunctionProblem() : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("Objectives", "The dimensionality of the solution vector (number of objectives).", new IntValue(2)));
      Parameters.Add(new ValueParameter<IMultiObjectiveTestFunction>("TestFunction", "The function that is to be optimized.", new Fonseca()));

      BestKnownFrontParameter.Hidden = true;
      BestKnownFrontParameter.ReadOnly = true;
      ReferencePointParameter.ReadOnly = true;

      UpdateParameterValues();
      InitializeOperators();
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      IntValueParameterChangeHandler.Create(ObjectivesParameter, ObjectivesOnChanged);
      ParameterChangeHandler<IMultiObjectiveTestFunction>.Create(TestFunctionParameter, TestFunctionOnChanged);
    }


    public override void Analyze(RealVector[] solutions, double[][] qualities, ResultCollection results, IRandom random) {
      base.Analyze(solutions, qualities, results, random);
      if (results.ContainsKey("Pareto Front"))
        ((DoubleMatrix)results["Pareto Front"].Value).SortableView = true;
    }

    /// <summary>
    /// Checks whether a given solution violates the contraints of this function.
    /// </summary>
    /// <param name="individual"></param>
    /// <returns>a double array that holds the distances that describe how much every contraint is violated (0 is not violated). If the current TestFunction does not have constraints an array of length 0 is returned</returns>
    public double[] CheckContraints(RealVector individual) {
      var constrainedTestFunction = (IConstrainedTestFunction)TestFunction;
      return constrainedTestFunction != null ? constrainedTestFunction.CheckConstraints(individual, Objectives) : new double[0];
    }

    public override double[] Evaluate(RealVector solution, IRandom random, CancellationToken cancellationToken) {
      return TestFunction.Evaluate(solution, Objectives);
    }


    public void Load(MOTFData data) {
      TestFunction = data.TestFunction;
    }

    #region Events
    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Dimension < TestFunction.MinimumSolutionLength || Dimension > TestFunction.MaximumSolutionLength)
        Dimension = Math.Min(TestFunction.MaximumSolutionLength, Math.Max(TestFunction.MinimumSolutionLength, Dimension));
      UpdateParameterValues();
      OnReset();
    }

    protected virtual void TestFunctionOnChanged() {
      Dimension = Math.Max(TestFunction.MinimumSolutionLength, Math.Min(Dimension, TestFunction.MaximumSolutionLength));
      Objectives = Math.Max(TestFunction.MinimumObjectives, Math.Min(Objectives, TestFunction.MaximumObjectives));
      UpdateParameterValues();
      OnReset();
    }

    protected virtual void ObjectivesOnChanged() {
      Objectives = Math.Min(TestFunction.MaximumObjectives, Math.Max(TestFunction.MinimumObjectives, Objectives));
      UpdateParameterValues();
      OnReset();
    }
    #endregion

    #region Helpers
    private void UpdateParameterValues() {
      Maximization = TestFunction.Maximization(Objectives);

      BestKnownFrontParameter.Value = DoubleMatrix.FromRows(TestFunction.OptimalParetoFront(Objectives));
      ReferencePoint = TestFunction.ReferencePoint(Objectives);

      BoundsRefParameter.Value = new DoubleMatrix(TestFunction.Bounds(Objectives));
    }

    private void InitializeOperators() {
      Operators.Add(new CrowdingAnalyzer());
      Operators.Add(new GenerationalDistanceAnalyzer());
      Operators.Add(new InvertedGenerationalDistanceAnalyzer());
      Operators.Add(new HypervolumeAnalyzer());
      Operators.Add(new SpacingAnalyzer());
      Operators.Add(new TimelineAnalyzer());
    }
    #endregion
  }
}