using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.AcceptanceMethods {
  [Item("GreedyAcceptance", " New solution is accepted if it reduces the costs compared to the current solution and is feasable")]
  [StorableType("07bb0acf-21a4-41c3-ae8d-34873244caf7")]
  internal class GreedyAcceptance : Item, ILNSAccept {
    #region Constructors and Cloning
    [StorableConstructor]
    protected GreedyAcceptance(StorableConstructorFlag _) : base(_) { }
    protected GreedyAcceptance(GreedyAcceptance original, Cloner cloner) : base(original, cloner) { }
    public GreedyAcceptance() { }
    public override IDeepCloneable Clone(Cloner cloner) { return new GreedyAcceptance(this, cloner); }
    #endregion
    public bool AcceptSolution(IVRPEncoding currentSolution, IVRPEncoding newSolution, IVRPProblemInstance vRPProblemInstance) {
      var evalCurrent = vRPProblemInstance.Evaluate(currentSolution);
      var evalNew = vRPProblemInstance.Evaluate(newSolution);

      return evalNew.Quality.CompareTo(evalCurrent.Quality) <= 0 && evalNew.Penalty == .0;
    }

  }
}
