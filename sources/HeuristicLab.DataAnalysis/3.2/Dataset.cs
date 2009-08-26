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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Globalization;
using System.Text;
using System.Linq;

namespace HeuristicLab.DataAnalysis {
  public sealed class Dataset : ItemBase {

    private string name;
    private double[] samples;
    private int rows;
    private int columns;
    private Dictionary<int, Dictionary<int, double>>[] cachedMeans;
    private Dictionary<int, Dictionary<int, double>>[] cachedRanges;
    private double[] scalingFactor;
    private double[] scalingOffset;
    private bool cachedValuesInvalidated = true;

    private bool fireChangeEvents = true;
    public bool FireChangeEvents {
      get { return fireChangeEvents; }
      set { fireChangeEvents = value; }
    }

    public string Name {
      get { return name; }
      set { name = value; }
    }

    public int Rows {
      get { return rows; }
      set { rows = value; }
    }

    public int Columns {
      get { return columns; }
      set {
        columns = value;
        if (variableNames == null || variableNames.Length != columns) {
          variableNames = new string[columns];
        }
      }
    }

    public double[] ScalingFactor {
      get { return scalingFactor; }
      set {
        if (value.Length != scalingFactor.Length)
          throw new ArgumentException("Length of scaling factor array doesn't match number of variables");
        scalingFactor = value;
      }
    }
    public double[] ScalingOffset {
      get { return scalingOffset; }
      set {
        if (value.Length != scalingOffset.Length)
          throw new ArgumentException("Length of scaling offset array doesn't match number of variables");
        scalingOffset = value;
      }
    }

    public double GetValue(int i, int j) {
      return samples[columns * i + j];
    }

    public void SetValue(int i, int j, double v) {
      if (v != samples[columns * i + j]) {
        samples[columns * i + j] = v;
        cachedValuesInvalidated = true;
        if (fireChangeEvents) FireChanged();
      }
    }

    public double[] Samples {
      get { return samples; }
      set {
        variableNames = Enumerable.Range(1, columns).Select(x => "Var" + x.ToString("###")).ToArray();
        scalingFactor = new double[columns];
        scalingOffset = new double[columns];
        for (int i = 0; i < scalingFactor.Length; i++) {
          scalingFactor[i] = 1.0;
          scalingOffset[i] = 0.0;
        }
        samples = value;
        cachedValuesInvalidated = true;
        if (fireChangeEvents) FireChanged();
      }
    }

    private string[] variableNames;
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

    public Dataset()
      : this(new double[,] { { 0.0 } }) {
    }

    public Dataset(double[,] samples) {
      Name = "-";
      Rows = samples.GetLength(0);
      Columns = samples.GetLength(1);
      double[] values = new double[Rows * Columns];
      int i = 0;
      for (int row = 0; row < Rows; row++) {
        for (int column = 0; column < columns; column++) {
          values[i++] = samples[row, column];
        }
      }
      Samples = values;
      fireChangeEvents = true;
    }


    public string GetVariableName(int variableIndex) {
      return variableNames[variableIndex];
    }

    public int GetVariableIndex(string variableName) {
      for (int i = 0; i < variableNames.Length; i++) {
        if (variableNames[i].Equals(variableName)) return i;
      }
      throw new ArgumentException("The variable name " + variableName + " was not found.");
    }

    public double[] GetVariableValues(int variableIndex, int start, int end) {
      if (start < 0 || !(start <= end))
        throw new ArgumentException("Start must be between 0 and end (" + end + ").");
      if (end > rows || end < start)
        throw new ArgumentException("End must be between start (" + start + ") and dataset rows (" + rows + ").");

      double[] values = new double[end - start];
      for (int i = 0; i < end - start; i++)
        values[i] = GetValue(i + start, variableIndex);
      return values;
    }

    public double[] GetVariableValues(int variableIndex) {
      return GetVariableValues(variableIndex, 0, this.rows);
    }

    public double[] GetVariableValues(string variableName, int start, int end) {
      return GetVariableValues(GetVariableIndex(variableName), start, end);
    }

    public double[] GetVariableValues(string variableName) {
      return GetVariableValues(variableName, 0, this.rows);
    }

    public void SetVariableName(int variableIndex, string name) {
      variableNames[variableIndex] = name;
    }

    public bool ContainsVariableName(string variableName) {
      return this.variableNames.Contains(variableName);
    }

    public override IView CreateView() {
      return new DatasetView(this);
    }

    #region persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Dataset clone = new Dataset();
      clonedObjects.Add(Guid, clone);
      double[] cloneSamples = new double[rows * columns];
      Array.Copy(samples, cloneSamples, samples.Length);
      clone.rows = rows;
      clone.columns = columns;
      clone.Samples = cloneSamples;
      clone.Name = Name;
      clone.variableNames = new string[variableNames.Length];
      Array.Copy(variableNames, clone.variableNames, variableNames.Length);
      Array.Copy(scalingFactor, clone.scalingFactor, columns);
      Array.Copy(scalingOffset, clone.scalingOffset, columns);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute problemName = document.CreateAttribute("Name");
      problemName.Value = Name;
      node.Attributes.Append(problemName);
      XmlAttribute dim1 = document.CreateAttribute("Dimension1");
      dim1.Value = rows.ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim1);
      XmlAttribute dim2 = document.CreateAttribute("Dimension2");
      dim2.Value = columns.ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim2);
      XmlAttribute variableNames = document.CreateAttribute("VariableNames");
      variableNames.Value = GetVariableNamesString();
      node.Attributes.Append(variableNames);
      XmlAttribute scalingFactorsAttribute = document.CreateAttribute("ScalingFactors");
      scalingFactorsAttribute.Value = GetString(scalingFactor);
      node.Attributes.Append(scalingFactorsAttribute);
      XmlAttribute scalingOffsetsAttribute = document.CreateAttribute("ScalingOffsets");
      scalingOffsetsAttribute.Value = GetString(scalingOffset);
      node.Attributes.Append(scalingOffsetsAttribute);
      node.InnerText = ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Name = node.Attributes["Name"].Value;
      rows = int.Parse(node.Attributes["Dimension1"].Value, CultureInfo.InvariantCulture.NumberFormat);
      columns = int.Parse(node.Attributes["Dimension2"].Value, CultureInfo.InvariantCulture.NumberFormat);

      variableNames = ParseVariableNamesString(node.Attributes["VariableNames"].Value);
      if (node.Attributes["ScalingFactors"] != null)
        scalingFactor = ParseDoubleString(node.Attributes["ScalingFactors"].Value);
      else {
        scalingFactor = new double[columns]; // compatibility with old serialization format
        for (int i = 0; i < scalingFactor.Length; i++) scalingFactor[i] = 1.0;
      }
      if (node.Attributes["ScalingOffsets"] != null)
        scalingOffset = ParseDoubleString(node.Attributes["ScalingOffsets"].Value);
      else {
        scalingOffset = new double[columns]; // compatibility with old serialization format
        for (int i = 0; i < scalingOffset.Length; i++) scalingOffset[i] = 0.0;
      }

      string[] tokens = node.InnerText.Split(';');
      if (tokens.Length != rows * columns) throw new FormatException();
      samples = new double[rows * columns];
      for (int row = 0; row < rows; row++) {
        for (int column = 0; column < columns; column++) {
          if (double.TryParse(tokens[row * columns + column], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out samples[row * columns + column]) == false) {
            throw new FormatException("Can't parse " + tokens[row * columns + column] + " as double value.");
          }
        }
      }
    }

    public override string ToString() {
      return ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for (int row = 0; row < rows; row++) {
        for (int column = 0; column < columns; column++) {
          builder.Append(";");
          builder.Append(samples[row * columns + column].ToString("r", format));
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }

    private string GetVariableNamesString() {
      string s = "";
      for (int i = 0; i < variableNames.Length; i++) {
        s += variableNames[i] + "; ";
      }

      if (variableNames.Length > 0) {
        s = s.TrimEnd(';', ' ');
      }
      return s;
    }
    private string GetString(double[] xs) {
      string s = "";
      for (int i = 0; i < xs.Length; i++) {
        s += xs[i].ToString("r", CultureInfo.InvariantCulture) + "; ";
      }

      if (xs.Length > 0) {
        s = s.TrimEnd(';', ' ');
      }
      return s;
    }

    private string[] ParseVariableNamesString(string p) {
      p = p.Trim();
      string[] tokens = p.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < tokens.Length; i++) tokens[i] = tokens[i].Trim();
      return tokens;
    }
    private double[] ParseDoubleString(string s) {
      s = s.Trim();
      string[] ss = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
      double[] xs = new double[ss.Length];
      for (int i = 0; i < xs.Length; i++) {
        xs[i] = double.Parse(ss[i], CultureInfo.InvariantCulture);
      }
      return xs;
    }
    #endregion

    public double GetMean(int column) {
      return GetMean(column, 0, Rows);
    }

    public double GetMean(int column, int from, int to) {
      if (cachedValuesInvalidated) CreateDictionaries();
      if (!cachedMeans[column].ContainsKey(from) || !cachedMeans[column][from].ContainsKey(to)) {
        double[] values = new double[to - from];
        for (int sample = from; sample < to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double mean = Statistics.Mean(values);
        if (!cachedMeans[column].ContainsKey(from)) cachedMeans[column][from] = new Dictionary<int, double>();
        cachedMeans[column][from][to] = mean;
        return mean;
      } else {
        return cachedMeans[column][from][to];
      }
    }

    public double GetRange(int column) {
      return GetRange(column, 0, Rows);
    }

    public double GetRange(int column, int from, int to) {
      if (cachedValuesInvalidated) CreateDictionaries();
      if (!cachedRanges[column].ContainsKey(from) || !cachedRanges[column][from].ContainsKey(to)) {
        double[] values = new double[to - from];
        for (int sample = from; sample < to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double range = Statistics.Range(values);
        if (!cachedRanges[column].ContainsKey(from)) cachedRanges[column][from] = new Dictionary<int, double>();
        cachedRanges[column][from][to] = range;
        return range;
      } else {
        return cachedRanges[column][from][to];
      }
    }

    public double GetMaximum(int column) {
      return GetMaximum(column, 0, Rows);
    }

    public double GetMaximum(int column, int start, int end) {
      double max = Double.NegativeInfinity;
      for (int i = start; i < end; i++) {
        double val = GetValue(i, column);
        if (!double.IsNaN(val) && val > max) max = val;
      }
      return max;
    }

    public double GetMinimum(int column) {
      return GetMinimum(column, 0, Rows);
    }

    public double GetMinimum(int column, int start, int end) {
      double min = Double.PositiveInfinity;
      for (int i = start; i < end; i++) {
        double val = GetValue(i, column);
        if (!double.IsNaN(val) && val < min) min = val;
      }
      return min;
    }

    internal void ScaleVariable(int column) {
      if (scalingFactor[column] == 1.0 && scalingOffset[column] == 0.0) {
        double min = GetMinimum(column);
        double max = GetMaximum(column);
        double range = max - min;
        if (range == 0) ScaleVariable(column, 1.0, -min);
        else ScaleVariable(column, 1.0 / range, -min);
      }
      cachedValuesInvalidated = true;
      if (fireChangeEvents) FireChanged();
    }

    internal void ScaleVariable(int column, double factor, double offset) {
      scalingFactor[column] = factor;
      scalingOffset[column] = offset;
      for (int i = 0; i < Rows; i++) {
        double origValue = samples[i * columns + column];
        samples[i * columns + column] = (origValue + offset) * factor;
      }
      cachedValuesInvalidated = true;
      if (fireChangeEvents) FireChanged();
    }

    internal void UnscaleVariable(int column) {
      if (scalingFactor[column] != 1.0 || scalingOffset[column] != 0.0) {
        for (int i = 0; i < rows; i++) {
          double scaledValue = samples[i * columns + column];
          samples[i * columns + column] = scaledValue / scalingFactor[column] - scalingOffset[column];
        }
        scalingFactor[column] = 1.0;
        scalingOffset[column] = 0.0;
      }
      cachedValuesInvalidated = true;
      if (fireChangeEvents) FireChanged();
    }

    private void CreateDictionaries() {
      // keep a means and ranges dictionary for each column (possible target variable) of the dataset.
      cachedMeans = new Dictionary<int, Dictionary<int, double>>[columns];
      cachedRanges = new Dictionary<int, Dictionary<int, double>>[columns];
      for (int i = 0; i < columns; i++) {
        cachedMeans[i] = new Dictionary<int, Dictionary<int, double>>();
        cachedRanges[i] = new Dictionary<int, Dictionary<int, double>>();
      }
      cachedValuesInvalidated = false;
    }
  }
}
