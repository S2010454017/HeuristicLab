using HeuristicLab.PluginInfrastructure;

namespace ALNS {
  [Plugin("ALNS", "1.0")]
  [PluginFile("ALNS.dll", PluginFileType.Assembly)]

  [PluginDependency("HeuristicLab.Analysis", "3.3")]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Operators", "3.3")]
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Parameters", "3.3")]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Random", "3.3")]
  [PluginDependency("HeuristicLab.Selection", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Instances", "3.3")]
  [PluginDependency("HeuristicLab.Problems.VehicleRouting", "3.4")]
  public class Plugin : PluginBase {
  }
}