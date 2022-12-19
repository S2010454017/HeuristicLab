using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.RepairHeuristics {
  [Item("RegretRepair", "Considers the 2 cheapest insertion")]
  [StorableType("23ddd4ae-ab49-4ca1-8808-cde4746906e0")]
  internal class RegretRepair : Item, ILNSRepair {
    #region Constructors and Cloning
    [StorableConstructor]
    protected RegretRepair(StorableConstructorFlag _) : base(_) { }
    protected RegretRepair(RegretRepair original, Cloner cloner) : base(original, cloner) {
    }
    public RegretRepair() {}

    public override IDeepCloneable Clone(Cloner cloner) { return new RegretRepair(this, cloner); }
    #endregion
    public PotvinEncoding RepairSolution(PotvinEncoding solution, IVRPProblemInstance vRPProblem) {
      return Repair(solution, vRPProblem);
    }

    public PotvinEncoding Repair(PotvinEncoding solution, IVRPProblemInstance vRPProblem) {
      PotvinEncoding potvinEncoding = (PotvinEncoding)solution.Clone();
      int numberOfInserts = 2;

      //holds for each city the n cheapest insertions
      var evaluations = new Dictionary<int, List<(double quality, int indexInTour)>>();
      Enumerable.Range(0, potvinEncoding.Tours.Count)
                 .ToList()
                 .ForEach(x => evaluations.Add(x, new List<(double quality, int indexInTour)>()));

      //check each unrouted city
      foreach (var city in potvinEncoding.Unrouted) {
        Enumerable.Range(0, potvinEncoding.Tours.Count)
                  .ToList()
                  .ForEach(idx => evaluations[idx].Clear());
        //for each city check each route 2 times
        for (int currentInsert = 0; currentInsert < numberOfInserts; ++currentInsert) {
          //find best insertion for each tour
          for (int tourIndex = 0; tourIndex < potvinEncoding.Tours.Count; tourIndex++) {
            var tour = potvinEncoding.Tours[tourIndex];
            var alreadyConsidered = -1;
            //check if some position is already considered
            if (evaluations[tourIndex].Count > 0) {
              alreadyConsidered = evaluations[tourIndex].First().indexInTour;
            }

            var place = potvinEncoding.FindBestInsertionPlace(tour, city, alreadyConsidered);
            tour.Stops.Insert(place, city);
            var evalTour = vRPProblem.EvaluateTour(tour, potvinEncoding);
            evaluations[tourIndex].Add((evalTour.Quality, place));
            tour.Stops.Remove(city);
          }
        }

        // 2 regret heuristic => can access costs on index 0 and 1 directly (exactly two entries)
        var best = evaluations.Select(e => (result: e, cost: Math.Abs(e.Value[0].quality - e.Value[1].quality)))
                              .OrderByDescending(e => e.cost)
                              .First()
                              .result;

        var bestIndex = best.Value.OrderBy(e => e.quality).First().indexInTour;

        potvinEncoding.Tours[best.Key].Stops.Insert(bestIndex, city);
      }
      potvinEncoding.Unrouted.Clear();
      potvinEncoding.Repair();
      return potvinEncoding;
    }
  }
}
