﻿using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {
  internal class CustomJsonWriter : JsonTextWriter {
    private bool isRangeArray = false;
    public override void WriteStartArray() {
      
      if (isRangeArray) base.Formatting = Formatting.None;
      base.WriteStartArray();
    }

    public override void WritePropertyName(string name) {
      base.Formatting = Formatting.Indented;
      base.WritePropertyName(name);
      isRangeArray = name == "Range" || name == "Default";
    }

    public override void WriteStartObject() {
      base.Formatting = Formatting.Indented;
      base.WriteStartObject();
    }

    public override void WriteEndObject() {
      base.Formatting = Formatting.Indented;
      base.WriteEndObject();
    }

    public CustomJsonWriter(TextWriter writer) : base(writer) { }

    public static string Serialize(JToken token) {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter sw = new StringWriter();
      CustomJsonWriter writer = new CustomJsonWriter(sw);
      writer.Formatting = Formatting.Indented;
      serializer.Serialize(writer, token);
      return sw.ToString();
    }
  }
}
