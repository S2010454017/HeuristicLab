using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.RepairHeuristics {
  [StorableType("380333dd-fc40-4211-9ed7-a3885264b203")]
  public interface ILNSRepair:IItem {
    PotvinEncoding RepairSolution(PotvinEncoding solution, IVRPProblemInstance vRPProblem);
  }
}
