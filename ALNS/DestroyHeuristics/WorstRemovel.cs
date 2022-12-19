using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.DestroyHeuristics {

  [Item("WorstRemovel", "Removes the cities with the highest cost")]
  [StorableType("580fc4b1-0721-4fe1-a155-e162b9a74ab9")]
  internal class WorstRemovel : Item, ILNSDestroy {
    private readonly double destroyGrade;
    private readonly IVRPProblemInstance problemInstance;

    #region Constructors and Cloning
    [StorableConstructor]
    public WorstRemovel(StorableConstructorFlag _) : base(_) { }

    public WorstRemovel(double destroyGrade, IVRPProblemInstance problemInstance) {
      this.destroyGrade = destroyGrade;
      this.problemInstance = problemInstance;
    }

    public WorstRemovel(WorstRemovel original, Cloner cloner) : base(original, cloner) {
      destroyGrade = original.destroyGrade;
      problemInstance = cloner.Clone(problemInstance);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new WorstRemovel(this, cloner);
    }
    #endregion

    #region Helper struct
    private struct TourEvaluation {
      public TourEvaluation(int city, int tourIndex, int indexInTour, double evaluation) {
        City = city;
        TourIndex = tourIndex;
        IndexInTour = indexInTour;
        Evaluation = evaluation;
      }

      public int City { get; private set; }
      public int TourIndex { get; private set; }
      public int IndexInTour { get; private set; }
      public double Evaluation { get; private set; }
    }
    #endregion
    public PotvinEncoding DestroySolution(PotvinEncoding solution) {
      return DestroySolutionStatic(solution, problemInstance, destroyGrade);
    }

    public static PotvinEncoding DestroySolutionStatic(PotvinEncoding solution, IVRPProblemInstance problemInstance,
                                                       double destroyGrade) {

      PotvinEncoding potvinEncoding = (PotvinEncoding)solution.Clone();
      IList<(int city, int index)> citiesWithIndexInTour;
      IList<TourEvaluation> evaluations = new List<TourEvaluation>();
      
      Tour tour = new Tour();
      int countToRemove = (int)Math.Floor(potvinEncoding.Cities * destroyGrade);

      var evaluationSolution = problemInstance.Evaluate(potvinEncoding).Quality;

      //check each city in each tour
      for (int i = 0; i < potvinEncoding.Tours.Count; i++) {

        tour = potvinEncoding.Tours[i];
        citiesWithIndexInTour = tour.Stops.Select((c, idx) => (c, idx)).ToList();
       
        //remove each city and check of the quality is impacted
        foreach ((int city, int index) in citiesWithIndexInTour) {
          tour.Stops.Remove(city);
          var eval = problemInstance.EvaluateTour(tour, potvinEncoding);
          evaluations.Add(new TourEvaluation(city: city, tourIndex: i, indexInTour: index, evaluation: eval.Quality));
          tour.Stops.Insert(index, city);
        }
      }

      var worstCities = evaluations.Select(e => new { difference = evaluationSolution - e.Evaluation, entry = e })
                                   .OrderByDescending(e => e.difference).Take(countToRemove).ToList();

      //remove the n worst cities from the tours
      foreach (var item in worstCities) {
        potvinEncoding.Tours[item.entry.TourIndex].Stops.Remove(item.entry.City);
        potvinEncoding.Unrouted.Add(item.entry.City);
      }

      return potvinEncoding;
    }
  }
}
