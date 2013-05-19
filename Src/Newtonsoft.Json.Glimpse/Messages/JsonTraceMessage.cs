using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Glimpse.Messages
{
  internal class JsonTraceMessage
  {
    public TimeSpan? Duration { get; set; }
    public DateTime MessageDate { get; set; }
    public string Message { get; set; }
    public TraceLevel Level { get; set; }
    public Exception Exception { get; set; }
    public JsonAction Action { get; set; }
    public int Ordinal { get; set; }
    public string JsonText { get; set; }
    public string Type { get; set; }
  }
}