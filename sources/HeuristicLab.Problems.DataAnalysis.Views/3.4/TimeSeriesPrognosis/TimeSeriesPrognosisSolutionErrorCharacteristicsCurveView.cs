﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Error Characteristics Curve")]
  [Content(typeof(ITimeSeriesPrognosisSolution))]
  public partial class TimeSeriesPrognosisSolutionErrorCharacteristicsCurveView : DataAnalysisSolutionEvaluationView {
    protected const string TrainingSamples = "Training";
    protected const string TestSamples = "Test";
    protected const string AllSamples = "All Samples";

    public TimeSeriesPrognosisSolutionErrorCharacteristicsCurveView()
      : base() {
      InitializeComponent();

      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.Items.Add(AllSamples);

      cmbSamples.SelectedIndex = 0;

      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].AxisX.Title = "Absolute Error";
      chart.ChartAreas[0].AxisX.Minimum = 0.0;
      chart.ChartAreas[0].AxisX.Maximum = 1.0;
      chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
      chart.ChartAreas[0].CursorX.Interval = 0.01;

      chart.ChartAreas[0].AxisY.Title = "Number of Samples";
      chart.ChartAreas[0].AxisY.Minimum = 0.0;
      chart.ChartAreas[0].AxisY.Maximum = 1.0;
      chart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.2;
      chart.ChartAreas[0].CursorY.Interval = 0.01;
    }

    public new ITimeSeriesPrognosisSolution Content {
      get { return (ITimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }
    public ITimeSeriesPrognosisProblemData ProblemData {
      get {
        if (Content == null) return null;
        return Content.ProblemData;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    protected virtual void Content_ModelChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ModelChanged, sender, e);
      else UpdateChart();
    }
    protected virtual void Content_ProblemDataChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ProblemDataChanged, sender, e);
      else {
        UpdateChart();
      }
    }
    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateChart();
    }

    protected virtual void UpdateChart() {
      chart.Series.Clear();
      chart.Annotations.Clear();
      if (Content == null) return;

      var originalValues = GetOriginalValues();
      var meanModelEstimatedValues = GetMeanModelEstimatedValues(originalValues);
      var meanModelResiduals = GetResiduals(originalValues, meanModelEstimatedValues);

      meanModelResiduals.Sort();
      chart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(meanModelResiduals.Last());
      chart.ChartAreas[0].CursorX.Interval = meanModelResiduals.First() / 100;

      Series meanModelSeries = new Series("Mean Model");
      meanModelSeries.ChartType = SeriesChartType.FastLine;
      UpdateSeries(meanModelResiduals, meanModelSeries);
      meanModelSeries.ToolTip = "Area over Curve: " + CalculateAreaOverCurve(meanModelSeries);
      chart.Series.Add(meanModelSeries);

      AddTimeSeriesPrognosisSolution(Content);
    }

    protected void AddTimeSeriesPrognosisSolution(ITimeSeriesPrognosisSolution solution) {
      if (chart.Series.Any(s => s.Name == solution.Name)) return;

      Series solutionSeries = new Series(solution.Name);
      solutionSeries.Tag = solution;
      solutionSeries.ChartType = SeriesChartType.FastLine;
      var estimatedValues = GetResiduals(GetOriginalValues(), GetPrognosedValues(solution));
      UpdateSeries(estimatedValues, solutionSeries);
      solutionSeries.ToolTip = "Area over Curve: " + CalculateAreaOverCurve(solutionSeries);
      chart.Series.Add(solutionSeries);
    }

    protected void UpdateSeries(List<double> residuals, Series series) {
      series.Points.Clear();
      residuals.Sort();

      series.Points.AddXY(0, 0);
      for (int i = 0; i < residuals.Count; i++) {
        var point = new DataPoint();
        if (residuals[i] > chart.ChartAreas[0].AxisX.Maximum) {
          point.XValue = chart.ChartAreas[0].AxisX.Maximum;
          point.YValues[0] = ((double)i) / residuals.Count;
          point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
          series.Points.Add(point);
          break;
        }

        point.XValue = residuals[i];
        point.YValues[0] = ((double)i + 1) / residuals.Count;
        point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
        series.Points.Add(point);
      }

      if (series.Points.Last().XValue < chart.ChartAreas[0].AxisX.Maximum) {
        var point = new DataPoint();
        point.XValue = chart.ChartAreas[0].AxisX.Maximum;
        point.YValues[0] = 1;
        point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
        series.Points.Add(point);
      }
    }

    protected IEnumerable<double> GetOriginalValues() {
      IEnumerable<double> originalValues;
      switch (cmbSamples.SelectedItem.ToString()) {
        case TrainingSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
          break;
        case TestSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndizes);
          break;
        case AllSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable);
          break;
        default:
          throw new NotSupportedException();
      }
      return originalValues;
    }

    protected IEnumerable<double> GetPrognosedValues(ITimeSeriesPrognosisSolution solution) {
      IEnumerable<double> prognosedValues;
      switch (cmbSamples.SelectedItem.ToString()) {
        case TrainingSamples:
          prognosedValues = solution.PrognosedTrainingValues;
          break;
        case TestSamples:
          prognosedValues = solution.PrognosedTestValues;
          break;
        case AllSamples:
          prognosedValues = solution.PrognosedValues;
          break;
        default:
          throw new NotSupportedException();
      }
      return prognosedValues;
    }

    protected IEnumerable<double> GetMeanModelEstimatedValues(IEnumerable<double> originalValues) {
      double averageTrainingTarget = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes).Average();
      return Enumerable.Repeat(averageTrainingTarget, originalValues.Count());
    }

    protected virtual List<double> GetResiduals(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues) {
      return originalValues.Zip(estimatedValues, (x, y) => Math.Abs(x - y)).ToList();
    }

    private double CalculateAreaOverCurve(Series series) {
      if (series.Points.Count < 1) throw new ArgumentException("Could not calculate area under curve if less than 1 data points were given.");

      double auc = 0.0;
      for (int i = 1; i < series.Points.Count; i++) {
        double width = series.Points[i].XValue - series.Points[i - 1].XValue;
        double y1 = 1 - series.Points[i - 1].YValues[0];
        double y2 = 1 - series.Points[i].YValues[0];

        auc += (y1 + y2) * width / 2;
      }

      return auc;
    }

    protected void cmbSamples_SelectedIndexChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)cmbSamples_SelectedIndexChanged, sender, e);
      else UpdateChart();
    }
  }
}
