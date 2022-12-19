using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace ALNS.RepairHeuristics {
  [Item("GreedyRepair", "Insert each item at the first improving position")]
  [StorableType("8ceaef8f-a935-4e9a-9a78-c993f77e7108")]
  internal class GreedyRepair : Item, ILNSRepair {
    #region Constructors and Cloning
    [StorableConstructor]
    protected GreedyRepair(StorableConstructorFlag _) : base(_) { }
    protected GreedyRepair(GreedyRepair original, Cloner cloner) : base(original, cloner) { }
    public GreedyRepair() { }
    public override IDeepCloneable Clone(Cloner cloner) { return new GreedyRepair(this, cloner); }
    #endregion
    public PotvinEncoding RepairSolution(PotvinEncoding solution, IVRPProblemInstance vRPProblem) {
      return Repair(solution, vRPProblem);
    }

    public static PotvinEncoding Repair(PotvinEncoding solution, IVRPProblemInstance problemInstance) {
      PotvinEncoding potvinEncoding = (PotvinEncoding)solution.Clone();
      var evaluation = new List<(VRPEvaluation eval, int index)>(potvinEncoding.Tours.Count);

      //check every unrouted city
      foreach (var city in potvinEncoding.Unrouted) {
        evaluation.Clear();
        for (int i = 0; i < potvinEncoding.Tours.Count; i++) {
          var tour = potvinEncoding.Tours[i];
          var place = potvinEncoding.FindBestInsertionPlace(tour, city);

          //temporary insert city and evaluate tour
          tour.Stops.Insert(place, city);
          var evalTour = problemInstance.EvaluateTour(tour, potvinEncoding);
          evaluation.Add((evalTour, place));
          tour.Stops.Remove(city);

        }
        var best = evaluation.Select((e, i) => (res: e.eval, info: (tour: i, place: e.index)))
                             .OrderBy(e => e.res.Quality)
                             .First().info;

        potvinEncoding.Tours[best.tour].Stops.Insert(best.place, city);
        
      }
      potvinEncoding.Unrouted.Clear();
      potvinEncoding.Repair();
      return potvinEncoding;
    }
  }
}
