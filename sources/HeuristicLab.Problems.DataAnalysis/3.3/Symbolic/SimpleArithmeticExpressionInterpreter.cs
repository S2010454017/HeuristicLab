﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Compiler;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SimpleArithmeticExpressionInterpreter", "Interpreter for arithmetic symbolic expression trees including function calls.")]
  // not thread safe!
  public sealed class SimpleArithmeticExpressionInterpreter : NamedItem, ISymbolicExpressionTreeInterpreter {
    private class OpCodes {
      public const byte Add = 1;
      public const byte Sub = 2;
      public const byte Mul = 3;
      public const byte Div = 4;

      public const byte Sin = 5;
      public const byte Cos = 6;
      public const byte Tan = 7;

      public const byte Log = 8;
      public const byte Exp = 9;

      public const byte IfThenElse = 10;

      public const byte GT = 11;
      public const byte LT = 12;

      public const byte AND = 13;
      public const byte OR = 14;
      public const byte NOT = 15;


      public const byte Average = 16;

      public const byte Call = 17;

      public const byte Variable = 18;
      public const byte LagVariable = 19;
      public const byte Constant = 20;
      public const byte Arg = 21;
    }

    private Dictionary<Type, byte> symbolToOpcode = new Dictionary<Type, byte>() {
      { typeof(Addition), OpCodes.Add },
      { typeof(Subtraction), OpCodes.Sub },
      { typeof(Multiplication), OpCodes.Mul },
      { typeof(Division), OpCodes.Div },
      { typeof(Sine), OpCodes.Sin },
      { typeof(Cosine), OpCodes.Cos },
      { typeof(Tangent), OpCodes.Tan },
      { typeof(Logarithm), OpCodes.Log },
      { typeof(Exponential), OpCodes.Exp },
      { typeof(IfThenElse), OpCodes.IfThenElse },
      { typeof(GreaterThan), OpCodes.GT },
      { typeof(LessThan), OpCodes.LT },
      { typeof(And), OpCodes.AND },
      { typeof(Or), OpCodes.OR },
      { typeof(Not), OpCodes.NOT},
      { typeof(Average), OpCodes.Average},
      { typeof(InvokeFunction), OpCodes.Call },
      { typeof(HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable), OpCodes.Variable },
      { typeof(LaggedVariable), OpCodes.LagVariable },
      { typeof(Constant), OpCodes.Constant },
      { typeof(Argument), OpCodes.Arg },
    };
    private const int ARGUMENT_STACK_SIZE = 1024;

    private Dataset dataset;
    private int row;
    private Instruction[] code;
    private int pc;
    private double[] argumentStack = new double[ARGUMENT_STACK_SIZE];
    private int argStackPointer;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private SimpleArithmeticExpressionInterpreter(bool deserializing) : base(deserializing) { }
    private SimpleArithmeticExpressionInterpreter(SimpleArithmeticExpressionInterpreter original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleArithmeticExpressionInterpreter(this, cloner);
    }

    public SimpleArithmeticExpressionInterpreter()
      : base() {
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(SymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      this.dataset = dataset;
      var compiler = new SymbolicExpressionTreeCompiler();
      compiler.AddInstructionPostProcessingHook(PostProcessInstruction);
      code = compiler.Compile(tree, MapSymbolToOpCode);
      foreach (var row in rows) {
        this.row = row;
        pc = 0;
        argStackPointer = 0;
        yield return Evaluate();
      }
    }

    private Instruction PostProcessInstruction(Instruction instr) {
      if (instr.opCode == OpCodes.Variable) {
        var variableTreeNode = instr.dynamicNode as VariableTreeNode;
        instr.iArg0 = (ushort)dataset.GetVariableIndex(variableTreeNode.VariableName);
      } else if (instr.opCode == OpCodes.LagVariable) {
        var variableTreeNode = instr.dynamicNode as LaggedVariableTreeNode;
        instr.iArg0 = (ushort)dataset.GetVariableIndex(variableTreeNode.VariableName);
      }
      return instr;
    }

    private byte MapSymbolToOpCode(SymbolicExpressionTreeNode treeNode) {
      if (symbolToOpcode.ContainsKey(treeNode.Symbol.GetType()))
        return symbolToOpcode[treeNode.Symbol.GetType()];
      else
        throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }

    private double Evaluate() {
      Instruction currentInstr = code[pc++];
      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            double s = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s += Evaluate();
            }
            return s;
          }
        case OpCodes.Sub: {
            double s = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s -= Evaluate();
            }
            if (currentInstr.nArguments == 1) s = -s;
            return s;
          }
        case OpCodes.Mul: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p *= Evaluate();
            }
            return p;
          }
        case OpCodes.Div: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p /= Evaluate();
            }
            if (currentInstr.nArguments == 1) p = 1.0 / p;
            return p;
          }
        case OpCodes.Average: {
            double sum = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              sum += Evaluate();
            }
            return sum / currentInstr.nArguments;
          }
        case OpCodes.Cos: {
            return Math.Cos(Evaluate());
          }
        case OpCodes.Sin: {
            return Math.Sin(Evaluate());
          }
        case OpCodes.Tan: {
            return Math.Tan(Evaluate());
          }
        case OpCodes.Exp: {
            return Math.Exp(Evaluate());
          }
        case OpCodes.Log: {
            return Math.Log(Evaluate());
          }
        case OpCodes.IfThenElse: {
            double condition = Evaluate();
            double result;
            if (condition > 0.0) {
              result = Evaluate(); SkipBakedCode();
            } else {
              SkipBakedCode(); result = Evaluate();
            }
            return result;
          }
        case OpCodes.AND: {
            double result = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result <= 0.0) SkipBakedCode();
              else {
                result = Evaluate();
              }
            }
            return result <= 0.0 ? -1.0 : 1.0;
          }
        case OpCodes.OR: {
            double result = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result > 0.0) SkipBakedCode();
              else {
                result = Evaluate();
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.NOT: {
            return -Evaluate();
          }
        case OpCodes.GT: {
            double x = Evaluate();
            double y = Evaluate();
            if (x > y) return 1.0;
            else return -1.0;
          }
        case OpCodes.LT: {
            double x = Evaluate();
            double y = Evaluate();
            if (x < y) return 1.0;
            else return -1.0;
          }
        case OpCodes.Call: {
            // evaluate sub-trees
            // push on argStack in reverse order 
            for (int i = 0; i < currentInstr.nArguments; i++) {
              argumentStack[argStackPointer + currentInstr.nArguments - i] = Evaluate();
            }
            argStackPointer += currentInstr.nArguments;

            // save the pc
            int nextPc = pc;
            // set pc to start of function  
            pc = currentInstr.iArg0;
            // evaluate the function
            double v = Evaluate();

            // decrease the argument stack pointer by the number of arguments pushed
            // to set the argStackPointer back to the original location
            argStackPointer -= currentInstr.nArguments;

            // restore the pc => evaluation will continue at point after my subtrees  
            pc = nextPc;
            return v;
          }
        case OpCodes.Arg: {
            return argumentStack[argStackPointer - currentInstr.iArg0];
          }
        case OpCodes.Variable: {
            var variableTreeNode = currentInstr.dynamicNode as VariableTreeNode;
            return dataset[row, currentInstr.iArg0] * variableTreeNode.Weight;
          }
        case OpCodes.LagVariable: {
            var lagVariableTreeNode = currentInstr.dynamicNode as LaggedVariableTreeNode;
            int actualRow = row + lagVariableTreeNode.Lag;
            if (actualRow < 0 || actualRow >= dataset.Rows) throw new ArgumentException("Out of range access to dataset row: " + row);
            return dataset[actualRow, currentInstr.iArg0] * lagVariableTreeNode.Weight;
          }
        case OpCodes.Constant: {
            var constTreeNode = currentInstr.dynamicNode as ConstantTreeNode;
            return constTreeNode.Value;
          }
        default: throw new NotSupportedException();
      }
    }

    // skips a whole branch
    private void SkipBakedCode() {
      int i = 1;
      while (i > 0) {
        i += code[pc++].nArguments;
        i--;
      }
    }
  }
}
