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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using System.Xml;

namespace HeuristicLab.Functions {
  class BakedFunctionTree : ItemBase, IFunctionTree {
    private List<int> code;
    private List<double> data;

    public BakedFunctionTree() {
      code = new List<int>();
      data = new List<double>();
    }

    internal BakedFunctionTree(IFunction function)
      : this() {
      code.Add(0);
      code.Add(BakedTreeEvaluator.MapFunction(function));
      code.Add(0);
      treesExpanded = true;
      subTrees = new List<IFunctionTree>();
      variables = new List<IVariable>();
      variablesExpanded = true;
      foreach(IVariableInfo variableInfo in function.VariableInfos) {
        if(variableInfo.Local) {
          variables.Add((IVariable)function.GetVariable(variableInfo.FormalName).Clone());
        }
      }
    }

    internal BakedFunctionTree(IFunctionTree tree)
      : this() {
      code.Add(0);
      code.Add(BakedTreeEvaluator.MapFunction(tree.Function));
      code.Add(tree.LocalVariables.Count);
      foreach(IVariable variable in tree.LocalVariables) {
        IItem value = variable.Value;
        data.Add(GetDoubleValue(value));
      }
      foreach(IFunctionTree subTree in tree.SubTrees) {
        AddSubTree(new BakedFunctionTree(subTree));
      }
    }

    private double GetDoubleValue(IItem value) {
      if(value is DoubleData) {
        return ((DoubleData)value).Data;
      } else if(value is ConstrainedDoubleData) {
        return ((ConstrainedDoubleData)value).Data;
      } else if(value is IntData) {
        return ((IntData)value).Data;
      } else if(value is ConstrainedIntData) {
        return ((ConstrainedIntData)value).Data;
      } else throw new NotSupportedException("Invalid datatype of local variable for GP");
    }

    private void BranchLength(int branchRoot, out int codeLength, out int dataLength) {
      int arity = code[branchRoot];
      int nLocalVariables = code[branchRoot + 2];
      codeLength = 3;
      dataLength = nLocalVariables;
      int subBranchStart = branchRoot + codeLength;
      for(int i = 0; i < arity; i++) {
        int branchCodeLength;
        int branchDataLength;
        BranchLength(subBranchStart, out branchCodeLength, out branchDataLength);
        subBranchStart += branchCodeLength;
        codeLength += branchCodeLength;
        dataLength += branchDataLength;
      }
    }

    private void FlattenTrees() {
      if(treesExpanded) {
        code[0] = subTrees.Count;
        foreach(BakedFunctionTree subTree in subTrees) {
          subTree.FlattenVariables();
          subTree.FlattenTrees();
          code.AddRange(subTree.code);
          data.AddRange(subTree.data);
        }
        treesExpanded = false;
        subTrees = null;
      }
    }

    private void FlattenVariables() {
      if(variablesExpanded) {
        code[2] = variables.Count;
        foreach(IVariable variable in variables) {
          data.Add(GetDoubleValue(variable.Value));
        }
        variablesExpanded = false;
        variables = null;
      }
    }

    private bool treesExpanded = false;
    private List<IFunctionTree> subTrees;
    public IList<IFunctionTree> SubTrees {
      get {
        if(!treesExpanded) {
          subTrees = new List<IFunctionTree>();
          int arity = code[0];
          int nLocalVariables = code[2];
          int branchIndex = 3;
          int dataIndex = nLocalVariables; // skip my local variables to reach the local variables of the first branch
          for(int i = 0; i < arity; i++) {
            BakedFunctionTree subTree = new BakedFunctionTree();
            int codeLength;
            int dataLength;
            BranchLength(branchIndex, out codeLength, out dataLength);
            subTree.code = code.GetRange(branchIndex, codeLength);
            subTree.data = data.GetRange(dataIndex, dataLength);
            branchIndex += codeLength;
            dataIndex += dataLength;
            subTrees.Add(subTree);
          }
          treesExpanded = true;
          code.RemoveRange(3, code.Count - 3);
          code[0] = 0;
          data.RemoveRange(nLocalVariables, data.Count - nLocalVariables);
        }
        return subTrees;
      }
    }

    private bool variablesExpanded = false;
    private List<IVariable> variables;
    public ICollection<IVariable> LocalVariables {
      get {
        if(!variablesExpanded) {
          variables = new List<IVariable>();
          IFunction function = BakedTreeEvaluator.MapSymbol(code[1]);
          int localVariableIndex = 0;
          foreach(IVariableInfo variableInfo in function.VariableInfos) {
            if(variableInfo.Local) {
              IVariable clone = (IVariable)function.GetVariable(variableInfo.FormalName).Clone();
              IItem value = clone.Value;
              if(value is ConstrainedDoubleData) {
                ((ConstrainedDoubleData)value).Data = data[localVariableIndex];
              } else if(value is ConstrainedIntData) {
                ((ConstrainedIntData)value).Data = (int)data[localVariableIndex];
              } else if(value is DoubleData) {
                ((DoubleData)value).Data = data[localVariableIndex];
              } else if(value is IntData) {
                ((IntData)value).Data = (int)data[localVariableIndex];
              } else throw new NotSupportedException("Invalid local variable type for GP.");
              variables.Add(clone);
              localVariableIndex++;
            }
          }
          variablesExpanded = true;
          code[2] = 0;
          data.RemoveRange(0, variables.Count);
        }
        return variables;
      }
    }

    public IFunction Function {
      get { return BakedTreeEvaluator.MapSymbol(code[1]); }
    }

    public IVariable GetLocalVariable(string name) {
      foreach(IVariable var in LocalVariables) {
        if(var.Name == name) return var;
      }
      return null;
    }

    public void AddVariable(IVariable variable) {
      throw new NotSupportedException();
    }

    public void RemoveVariable(string name) {
      throw new NotSupportedException();
    }

    public void AddSubTree(IFunctionTree tree) {
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.Add(tree);
    }

    public void InsertSubTree(int index, IFunctionTree tree) {
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.Insert(index, tree);
    }

    public void RemoveSubTree(int index) {
      // sanity check
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.RemoveAt(index);
    }

    public double Evaluate(Dataset dataset, int sampleIndex) {
      FlattenVariables();
      FlattenTrees();
      return BakedTreeEvaluator.Evaluate(dataset, sampleIndex, code, data);
    }


    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Function", Function, document, persistedObjects));
      XmlNode subTreesNode = document.CreateNode(XmlNodeType.Element, "SubTrees", null);
      for(int i = 0; i < SubTrees.Count; i++)
        subTreesNode.AppendChild(PersistenceManager.Persist(SubTrees[i], document, persistedObjects));
      node.AppendChild(subTreesNode);
      XmlNode variablesNode = document.CreateNode(XmlNodeType.Element, "Variables", null);
      foreach(IVariable variable in LocalVariables)
        variablesNode.AppendChild(PersistenceManager.Persist(variable, document, persistedObjects));
      node.AppendChild(variablesNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      IFunction function = (IFunction)PersistenceManager.Restore(node.SelectSingleNode("Function"), restoredObjects);
      code.Add(0);
      code.Add(BakedTreeEvaluator.MapFunction(function));
      code.Add(0);
      treesExpanded = true;
      subTrees = new List<IFunctionTree>();
      variables = new List<IVariable>();
      variablesExpanded = true;
      XmlNode subTreesNode = node.SelectSingleNode("SubTrees");
      for(int i = 0; i < subTreesNode.ChildNodes.Count; i++)
        subTrees.Add((IFunctionTree)PersistenceManager.Restore(subTreesNode.ChildNodes[i], restoredObjects));
      XmlNode variablesNode = node.SelectSingleNode("Variables");
      foreach(XmlNode variableNode in variablesNode.ChildNodes)
        variables.Add((IVariable)PersistenceManager.Restore(variableNode, restoredObjects));
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      BakedFunctionTree clone = new BakedFunctionTree();
      // in case the user (de)serialized the tree between evaluation and selection we have to flatten the tree again.
      if(treesExpanded) FlattenTrees(); 
      if(variablesExpanded) FlattenVariables();
      clone.code.AddRange(code);
      clone.data.AddRange(data);
      return clone;
    }

    public override IView CreateView() {
      return new FunctionTreeView(this);
    }
  }
}
