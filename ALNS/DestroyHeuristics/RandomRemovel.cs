using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.DestroyHeuristics {
  [Item("RandomRemovel", "Randomly select cities to remove from solution")]
  [StorableType("2bc2bb32-7f43-48d2-b97f-eeb7da151d36")]
  internal class RandomRemovel : Item, ILNSDestroy {

    private readonly IRandom random;
    private readonly double destroyGrade;
    private readonly IVRPProblemInstance problemInstance;

    #region Constructors and Cloning
    [StorableConstructor]
    public RandomRemovel(StorableConstructorFlag _) : base(_) { }

    public RandomRemovel(IRandom random, double destroyGrade, IVRPProblemInstance problemInstance) {
      this.random = random;
      this.destroyGrade = destroyGrade;
      this.problemInstance = problemInstance;
    }

    public RandomRemovel(RandomRemovel original, Cloner cloner) : base(original, cloner) {
      random = cloner.Clone(original.random);
      destroyGrade = original.destroyGrade;
      problemInstance = cloner.Clone(problemInstance);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomRemovel(this, cloner);
    }
    #endregion
    public PotvinEncoding DestroySolution(PotvinEncoding solution) {
      return RandomRemovel.DestroySolutionStatic(solution, problemInstance, random, destroyGrade);
    }

    public static PotvinEncoding DestroySolutionStatic(PotvinEncoding solution, IVRPProblemInstance problemInstance,
                                                    IRandom random, double destroyGrade) {

      PotvinEncoding potvinEncoding = (PotvinEncoding)solution.Clone();
      int countToRemove = (int)Math.Floor(potvinEncoding.Cities * destroyGrade);

      while (countToRemove > 0) {
        //select random number; cities start with index 1 (index 0 is depot)
        int cityIndex = random.Next(1, potvinEncoding.Cities);
        
        //cant remove a city twice => next random
        while (potvinEncoding.Unrouted.Contains(cityIndex)) {
          cityIndex = random.Next(1, potvinEncoding.Cities);
        }

        int tourIndex = potvinEncoding.Tours.FindIndex(t => t.Stops.Contains(cityIndex));
        if (tourIndex > -1) {
          potvinEncoding.Tours[tourIndex].Stops.Remove(cityIndex);
          --countToRemove;
          potvinEncoding.Unrouted.Add(cityIndex);
        }
      }
      return potvinEncoding;
    }
  }
}
