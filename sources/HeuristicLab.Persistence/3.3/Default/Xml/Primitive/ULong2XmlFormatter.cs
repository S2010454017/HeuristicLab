﻿using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {  
  
  [EmptyStorableClass]
  public class ULong2XmlFormatter : SimpleNumber2XmlFormatterBase<ulong> { }  
  
}