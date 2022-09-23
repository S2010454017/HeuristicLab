#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;
using HeuristicLab.PluginInfrastructure.Starter;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Static class that contains the main entry point of the plugin infrastructure.
  /// </summary>
  public static class Main {
    /// <summary>
    /// Main entry point of the plugin infrastructure. Loads the starter form.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Run(string[] args) {
      if ((!FrameworkVersionErrorDialog.NET4_5Installed && !FrameworkVersionErrorDialog.MonoInstalled)
        || (FrameworkVersionErrorDialog.MonoInstalled && !FrameworkVersionErrorDialog.MonoCorrectVersionInstalled)) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrameworkVersionErrorDialog());
      } else {
        try {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new StarterForm(args));
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(ex);
        }
      }
    }

    public static void HeadlessRun(string[] args) {
      try {
        string pluginPath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        var pluginManager = new PluginManager(pluginPath);
        pluginManager.DiscoverAndCheckPlugins();

        var arguments = CommandLineArgumentHandling.GetArguments(args);
        foreach (var argument in arguments) {
          if (argument is StartArgument) {
            var arg = (StartArgument)argument;
            var appDesc = (from desc in pluginManager.Applications
                           where desc.Name.Equals(arg.Value)
                           select desc).SingleOrDefault();
            if (appDesc != null) {
              pluginManager.Run(appDesc, arguments);
            }
          }
        }
      } catch (Exception e) {
        Console.Error.WriteLine($"{e.Message} \n\n {e.StackTrace}");
        Environment.Exit(-1);
      }
    }
  }
}
