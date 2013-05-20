using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Core.Setting;

namespace Newtonsoft.Json.Glimpse
{
  /// <summary>
  /// Provides Json.NET Glimpse extension helper methods.
  /// </summary>
  public static class GlimpseJson
  {
    /// <summary>
    /// Initializes the Json.NET Glimpse extension.
    /// </summary>
    public static void Initialize()
    {
#pragma warning disable 612,618
      Initialize(GlimpseConfiguration.GetConfiguredMessageBroker(), GlimpseConfiguration.GetConfiguredTimerStrategy());
#pragma warning restore 612,618
    }

    internal static void Initialize(IMessageBroker messageBroker, Func<IExecutionTimer> timerStrategy)
    {
      Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
      if (defaultSettings != null)
      {
        // ensure any existing trace writer is wrapped
        JsonConvert.DefaultSettings = () =>
        {
          JsonSerializerSettings existingSettings = defaultSettings() ?? new JsonSerializerSettings();

          // anti-inception
          if (!(existingSettings.TraceWriter is GlimpseTraceWriter))
            existingSettings.TraceWriter = new GlimpseTraceWriter(messageBroker, timerStrategy, existingSettings.TraceWriter);

          return existingSettings;
        };
      }
      else
      {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
          TraceWriter = new GlimpseTraceWriter(messageBroker, timerStrategy)
        };
      }
    }
  }
}