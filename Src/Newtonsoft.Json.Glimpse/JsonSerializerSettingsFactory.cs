using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;

namespace Newtonsoft.Json.Glimpse
{
  internal class JsonSerializerSettingsFactory
  {
    private readonly Func<JsonSerializerSettings> _existingDefaultSettings;
    private readonly Func<RuntimePolicy> _runtimePolicyStrategy;
    private readonly Func<IExecutionTimer> _timerStrategy;
    private readonly IMessageBroker _messageBroker;

    public JsonSerializerSettingsFactory(
      Func<JsonSerializerSettings> existingDefaultSettings,
      Func<RuntimePolicy> runtimePolicyStrategy,
      Func<IExecutionTimer> timerStrategy,
      IMessageBroker messageBroker)
    {
      _existingDefaultSettings = existingDefaultSettings;
      _runtimePolicyStrategy = runtimePolicyStrategy;
      _timerStrategy = timerStrategy;
      _messageBroker = messageBroker;
    }

    private bool IsGlimpseEnabled()
    {
      RuntimePolicy runtimePolicy = _runtimePolicyStrategy();
      return (runtimePolicy >= RuntimePolicy.PersistResults);
    }

    public JsonSerializerSettings GetDefaultSerializerSettings()
    {
      if (_existingDefaultSettings != null)
      {
        // ensure any existing settings are preserved
        // and the existing trace writer is wrapped
        JsonSerializerSettings existingSettings = _existingDefaultSettings() ?? new JsonSerializerSettings();

        if (!IsGlimpseEnabled())
          return existingSettings;

        // anti-inception
        if (!(existingSettings.TraceWriter is GlimpseTraceWriter))
          existingSettings.TraceWriter = new GlimpseTraceWriter(_messageBroker, _timerStrategy, existingSettings.TraceWriter);

        return existingSettings;
      }
      else
      {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        if (!IsGlimpseEnabled())
          return settings;

        settings.TraceWriter = new GlimpseTraceWriter(_messageBroker, _timerStrategy);

        return settings;
      }
    }
  }
}