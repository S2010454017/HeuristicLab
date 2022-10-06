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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Programmable {
  public abstract class CompiledProblemDefinition<TEncoding, TEncodedSolution> : IProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {
    private TEncoding encoding;
    public TEncoding Encoding {
      get { return encoding; }
      internal set {
        if (value == null) throw new ArgumentNullException("The encoding must not be null.");
        encoding = value;
      }
    }

    public dynamic vars { get; set; }
    public abstract void Initialize();

    protected CompiledProblemDefinition() { }
    protected CompiledProblemDefinition(TEncoding encoding)
      : base() {
      Encoding = encoding;
    }
  }

  public abstract class CompiledSingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> : CompiledProblemDefinition<TEncoding, TEncodedSolution>, ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {
    protected CompiledSingleObjectiveProblemDefinition() : base() { }

    protected CompiledSingleObjectiveProblemDefinition(TEncoding encoding)
      : base(encoding) { }

    #region ISingleObjectiveProblemDefinition<TEncoding,TEncodedSolution> Members
    public abstract bool Maximization { get; }

    public virtual ISingleObjectiveEvaluationResult Evaluate(TEncodedSolution solution, IRandom random) {
      return Evaluate(solution, random, CancellationToken.None);
    }
    public abstract ISingleObjectiveEvaluationResult Evaluate(TEncodedSolution solution, IRandom random, CancellationToken cancellationToken);

    public virtual void Evaluate(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random) {
      Evaluate(solutionContext, random, CancellationToken.None);
    }
    public virtual void Evaluate(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random, CancellationToken cancellationToken) {
      var evaluationResult = Evaluate(solutionContext.EncodedSolution, random, cancellationToken);
      solutionContext.EvaluationResult = evaluationResult;
    }


    public virtual void Analyze(ISingleObjectiveSolutionContext<TEncodedSolution>[] solutionContexts, IRandom random) { }

    public virtual IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution solutions, IRandom random) {
      return Enumerable.Empty<TEncodedSolution>();
    }
    public virtual IEnumerable<ISingleObjectiveSolutionContext<TEncodedSolution>> GetNeighbors(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random) {
      return GetNeighbors(solutionContext.EncodedSolution, random).Select(n => new SingleObjectiveSolutionContext<TEncodedSolution>(n));
    }

    public bool IsBetter(double quality, double bestQuality) {
      return Maximization ? quality > bestQuality : quality < bestQuality;
    }
    #endregion
  }

  public abstract class CompiledMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution> : CompiledProblemDefinition<TEncoding, TEncodedSolution>, IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {
    protected CompiledMultiObjectiveProblemDefinition() : base() { }

    protected CompiledMultiObjectiveProblemDefinition(TEncoding encoding)
      : base(encoding) { }

    #region IMultiObjectiveProblemDefinition<TEncoding,TEncodedSolution> Members
    public int Objectives => Maximization.Length;
    public abstract bool[] Maximization { get; }
    public abstract IReadOnlyList<double[]> BestKnownFront { get; }
    public abstract double[] ReferencePoint { get; }

    public virtual double[] Evaluate(TEncodedSolution solution, IRandom random) {
      return Evaluate(solution, random, CancellationToken.None);
    }
    public abstract double[] Evaluate(TEncodedSolution solution, IRandom random, CancellationToken cancellationToken);
    public abstract void Analyze(TEncodedSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random);
    public abstract IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution individual, IRandom random);
    #endregion
  }
}