﻿using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Default.Decomposers.Storable;
using System.Globalization;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {
  
  [EmptyStorableClass]
  public class Guid2XmlFormatter : PrimitiveXmlFormatterBase<Guid> {

    public override XmlString Format(Guid o) {
      return new XmlString(o.ToString("D", CultureInfo.InvariantCulture));
    }

    public override Guid Parse(XmlString t) {
      try {
        return new Guid(t.Data);
      } catch (FormatException x) {
        throw new PersistenceException("Cannot parse Guid string representation.", x);
      } catch (OverflowException x) {
        throw new PersistenceException("Overflow during Guid parsing.", x);
      }
    }
  }  
}