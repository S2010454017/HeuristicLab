using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace ALNS.AcceptanceMethods {
  [StorableType("7da3cc62-6e3a-4ca2-8c86-e95c00955efc")]
  public interface ILNSAccept:IItem {
    bool AcceptSolution(IVRPEncoding currentSolution, IVRPEncoding newSolution, IVRPProblemInstance vRPProblemInstance);
  }
}
