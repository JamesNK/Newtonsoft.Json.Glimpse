#region License
// Copyright (c) 2013 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

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