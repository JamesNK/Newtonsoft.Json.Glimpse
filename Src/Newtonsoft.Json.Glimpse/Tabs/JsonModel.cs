using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Glimpse.Messages;

namespace Newtonsoft.Json.Glimpse.Tabs
{
  internal class JsonModel
  {
    public int Ordinal { get; set; }
    public JsonAction Action { get; set; }
    public TraceLevel Level { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public string JsonText { get; set; }
    public Exception Exception { get; set; }
    public TimeSpan? Duration { get; set; }

    public JsonModel(JsonTraceMessage message)
    {
      Ordinal = message.Ordinal;
      Action = message.Action;
      Level = message.Level;
      Message = message.Message;
      Type = message.Type;
      JsonText = message.JsonText;
      Exception = message.Exception;
      Duration = message.Duration;
    }
  }
}