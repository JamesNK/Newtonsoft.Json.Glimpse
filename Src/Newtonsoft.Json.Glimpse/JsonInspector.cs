using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework.Support;

namespace Newtonsoft.Json.Glimpse
{
  internal class JsonInspector : IInspector
  {
    public void Setup(IInspectorContext context)
    {
      Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
      if (defaultSettings != null)
      {
        // ensure any existing trace writer is wrapped
        JsonConvert.DefaultSettings = () =>
        {
          JsonSerializerSettings existingSettings = defaultSettings() ?? new JsonSerializerSettings();
          existingSettings.TraceWriter = new GlimpseTraceWriter(context.MessageBroker, context.TimerStrategy, existingSettings.TraceWriter);

          return existingSettings;
        };
      }
      else
      {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
          TraceWriter = new GlimpseTraceWriter(context.MessageBroker, context.TimerStrategy)
        };
      }
    }
  }
}