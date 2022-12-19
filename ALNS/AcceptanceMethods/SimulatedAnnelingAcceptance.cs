using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Random;

namespace ALNS.AcceptanceMethods {
  [Item("SimulatedAnnelingAcceptance", "New solution is accepted according to the simulated anneling criteria")]
  [StorableType("52df1bce-5898-4fad-826a-9a0fc40caf55")]
  internal class SimulatedAnnelingAcceptance : Item, ILNSAccept {
    [Storable]
    private readonly IRandom random = new MersenneTwister();
    [Storable]
    private readonly double omega;
    [Storable]
    private double temperature;

    #region Constructors and Cloning
    [StorableConstructor]
    protected SimulatedAnnelingAcceptance(StorableConstructorFlag _) : base(_) { }
    protected SimulatedAnnelingAcceptance(SimulatedAnnelingAcceptance original, Cloner cloner) : base(original, cloner) {
      random = cloner.Clone(original.random);
      omega = original.omega;
      temperature = original.temperature;
    }
    public SimulatedAnnelingAcceptance(double omega = 0.75, double temperature = 1200) {
      this.omega = omega;
      this.temperature = temperature;
    }

    private SimulatedAnnelingAcceptance() {}

    public override IDeepCloneable Clone(Cloner cloner) { return new SimulatedAnnelingAcceptance(this, cloner); }
    #endregion

    public bool AcceptSolution(IVRPEncoding currentSolution, IVRPEncoding newSolution, IVRPProblemInstance vRPProblemInstance) {
      var evalCurrent = vRPProblemInstance.Evaluate(currentSolution);
      var evalNew = vRPProblemInstance.Evaluate(newSolution);

      if (evalNew.Penalty != .0) {
        return false;
      }

      if (evalNew.Quality.CompareTo(evalCurrent.Quality) <= 0) {
        return true;
      }
      var result = Math.Exp(-(evalNew.Quality - evalCurrent.Quality) / temperature);
      temperature *= omega;
      return random.NextDouble() <= result;
    }
  }
}
