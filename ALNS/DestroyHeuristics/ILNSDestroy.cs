using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;

namespace ALNS.DestroyHeuristics {

  [StorableType("24f00781-b263-4ec3-851d-d0ed5446afbe")]
  public interface ILNSDestroy : IItem {
    PotvinEncoding DestroySolution(PotvinEncoding solution);
  }
}
