﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HEAL.Attic;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public readonly struct InstantiatorResult {
    public InstantiatorResult(IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItems) {
      Optimizer = optimizer;
      ConfiguredResultItems = configuredResultItems;
      PostProcessors = Enumerable.Empty<IResultCollectionPostProcessor>();
    }

    public InstantiatorResult(IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItems, IEnumerable<IResultCollectionPostProcessor> postProcessors) {
      Optimizer = optimizer;
      ConfiguredResultItems = configuredResultItems;
      PostProcessors = postProcessors;
    }

    public IOptimizer Optimizer { get; }
    public IEnumerable<IResultJsonItem> ConfiguredResultItems { get; }
    public IEnumerable<IResultCollectionPostProcessor> PostProcessors { get; }
  }


  /// <summary>
  /// Class to instantiate an IAlgorithm object with a json interface template and config.
  /// </summary>
  public class JsonTemplateInstantiator {

    #region Constants
    private const string RelativePathCurrentDirectoryRegex = @"^(\.)";
    #endregion

    #region Private Properties
    private JToken Template { get; set; }
    private JArray Config { get; set; }
    private IDictionary<string, IJsonItem> Objects { get; set; } = new Dictionary<string, IJsonItem>();
    #endregion

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IOptimizer object</returns>
    public static InstantiatorResult Instantiate(string templateFile, string configFile = null) {
      JsonTemplateInstantiator instantiator = new JsonTemplateInstantiator();
      return instantiator.ExecuteInstantiaton(templateFile, configFile);
    }

    #region Helper
    private InstantiatorResult ExecuteInstantiaton(string templateFile, string configFile = null) {

      #region Parse Files
      string templateFileFullPath = Path.GetFullPath(templateFile);
      Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        Config = JArray.Parse(File.ReadAllText(Path.GetFullPath(configFile)));
      #endregion

      // extract metadata information
      string relativePath = Template[Constants.Metadata][Constants.HLFileLocation].ToString().Trim(); // get relative path
      // convert to absolute path
      if (Regex.IsMatch(relativePath, RelativePathCurrentDirectoryRegex))
        relativePath = relativePath.Remove(0, 2); // remove first 2 chars -> indicates the current directory

      string hLFileLocation = Path.Combine(Path.GetDirectoryName(templateFileFullPath), relativePath);
      #region Deserialize HL File
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      IOptimizer optimizer = (IOptimizer)serializer.Deserialize(hLFileLocation);
      #endregion

      // collect all parameterizedItems from template
      CollectParameterizedItems(optimizer);
      
      if (Config != null)
        MergeTemplateWithConfig();

      // get algorithm root item
      IJsonItem rootItem = Objects.First().Value;

      // validation
      ValidationResult validationResult = rootItem.GetValidator().Validate();
      if (!validationResult.Success)
        throw validationResult.GenerateException();

      // inject configuration
      JsonItemConverter.Inject(optimizer, rootItem);

      IList<IResultCollectionPostProcessor> postProcessorList = new List<IResultCollectionPostProcessor>();
      var postProcessors = ApplicationManager.Manager.GetInstances<IResultCollectionPostProcessor>();
      foreach (JObject obj in Template["PostProcessors"]) {
        //string name = obj.Property("Name").Value.ToString();
        foreach(var proc in postProcessors) {
          if (proc.GetType().Name == obj["Name"].ToString())
            postProcessorList.Add(proc);
        }
      }

      return new InstantiatorResult(optimizer, CollectResults(), postProcessorList);
    }

    private IEnumerable<IResultJsonItem> CollectResults() {
      IList<IResultJsonItem> res = new List<IResultJsonItem>();
      foreach(JObject obj in Template[Constants.Results]) {
        //string name = obj.Property("Name").Value.ToString();
        var resultItem = new ResultJsonItem();
        resultItem.SetJObject(obj);
        res.Add(resultItem);
        //res.Add(new ResultJsonItem() { Name = name });
      }
      return res;
    }

    private void CollectParameterizedItems(IOptimizer optimizer) {
      IJsonItem root = JsonItemConverter.Extract(optimizer);
      Objects.Add(root.Path, root);

      foreach (JObject obj in Template[Constants.Parameters]) {
        string path = obj.Property("Path").Value.ToString();
        foreach(var tmp in root) {
          if(tmp.Path == path) {
            tmp.SetJObject(obj);
            Objects.Add(tmp.Path, tmp);
          }
        }
      }
    }
    
    private void MergeTemplateWithConfig() {
      foreach (JObject obj in Config) {
        // build item from config object
        var prop = obj.Property("Path");
        if (prop == null) 
          throw new ArgumentException("Path property is missing.");
        string path = prop.Value.ToString();
        // override default value
        if (Objects.TryGetValue(path, out IJsonItem param)) {
          // remove fixed template parameter from config => dont allow to copy them from concrete config
          // TODO: shift this into JsonItems?
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Minimum))?.Remove();
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Maximum))?.Remove();
          obj.Property(nameof(IConcreteRestrictedJsonItem<string>.ConcreteRestrictedItems))?.Remove();
          obj.Property(nameof(IMatrixJsonItem.ColumnsResizable))?.Remove();
          obj.Property(nameof(IMatrixJsonItem.RowsResizable))?.Remove();
          obj.Property(nameof(IArrayJsonItem.Resizable))?.Remove();
          // merge
          param.SetJObject(obj);
        } else throw new InvalidDataException($"No parameter with path='{path}' defined!");
      }
    }
    #endregion
  }
}
