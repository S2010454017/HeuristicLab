﻿using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {  

  [EmptyStorableClass]
  public class Long2XmlFormatter : SimpleNumber2XmlFormatterBase<long> { }
  
}