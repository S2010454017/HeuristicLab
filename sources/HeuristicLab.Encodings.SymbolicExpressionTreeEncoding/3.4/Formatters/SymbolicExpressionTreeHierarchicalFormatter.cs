﻿
using System.IO;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [Item("Hierarchical Formatter", "Formatter for symbolic expression trees that uses special characters for drawing a tree in text-mode.")]
  public sealed class SymbolicExpressionTreeHierarchicalFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    private SymbolicExpressionTreeHierarchicalFormatter(SymbolicExpressionTreeHierarchicalFormatter original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeHierarchicalFormatter(this, cloner);
    }

    public SymbolicExpressionTreeHierarchicalFormatter() :
      base("Hierarchical Formatter", "Formatter for symbolic expression trees that uses special characters for drawing a tree in text-mode.") { }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      var sw = new StringWriter();
      RenderTree(sw, symbolicExpressionTree);
      return sw.ToString();
    }

    private static void RenderTree(TextWriter writer, ISymbolicExpressionTree tree) {
      RenderNode(writer, tree.Root, string.Empty);
    }

    public static void RenderNode(TextWriter writer, ISymbolicExpressionTreeNode node, string prefix) {
      string label = node.ToString();
      writer.Write(label);
      if (node.SubtreeCount > 0) {
        var padding = prefix + new string(' ', label.Length);
        for (int i = 0; i != node.SubtreeCount; ++i) {
          char connector, extender = ' ';
          if (i == 0) {
            if (node.SubtreeCount > 1) {
              connector = RenderChars.JunctionDown;
              extender = RenderChars.VerticalLine;
            } else {
              connector = RenderChars.HorizontalLine;
              extender = ' ';
            }
          } else {
            writer.Write(padding);
            if (i == node.SubtreeCount - 1) {
              connector = RenderChars.CornerRight;
              extender = ' ';
            } else {
              connector = RenderChars.JunctionRight;
              extender = RenderChars.VerticalLine;
            }
          }
          writer.Write(string.Concat(connector, RenderChars.HorizontalLine));
          var newPrefix = string.Concat(padding, extender, ' ');
          RenderNode(writer, node.GetSubtree(i), newPrefix);
        }
      } else
        writer.WriteLine();
    }

    // helper class providing characters for displaying a tree in the console
    public static class RenderChars {
      public const char JunctionDown = '┬';
      public const char HorizontalLine = '─';
      public const char VerticalLine = '│';
      public const char JunctionRight = '├';
      public const char CornerRight = '└';
    }
  }
}
