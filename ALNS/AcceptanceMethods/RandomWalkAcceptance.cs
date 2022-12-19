using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.AcceptanceMethods {
  [Item("RandomWalkAcceptance", "Every new feasable solution is accepted.")]
  [StorableType("9a719cbf-4fb4-419c-8a5a-681351ec576e")]
  internal class RandomWalkAcceptance : Item, ILNSAccept {

    #region Constructors and Cloning
    [StorableConstructor]
    protected RandomWalkAcceptance(StorableConstructorFlag _) : base(_) { }
    protected RandomWalkAcceptance(RandomWalkAcceptance original, Cloner cloner) : base(original, cloner) { }
    public RandomWalkAcceptance() { }
    public override IDeepCloneable Clone(Cloner cloner) { return new RandomWalkAcceptance(this, cloner); }
    #endregion

    public bool AcceptSolution(IVRPEncoding currentSolution, IVRPEncoding newSolution, IVRPProblemInstance vRPProblemInstance) {
      return vRPProblemInstance.Evaluate(newSolution).Penalty == .0;
    }
  }
}
