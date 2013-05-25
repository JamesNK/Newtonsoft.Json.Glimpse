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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using Newtonsoft.Json.Glimpse.Messages;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Glimpse
{
  internal class GlimpseTraceWriter : ITraceWriter
  {
    private readonly IMessageBroker _messageBroker;
    private readonly Func<IExecutionTimer> _timerStrategy;
    private readonly ITraceWriter _innerTraceWriter;
    private readonly IList<JsonTraceMessage> _traceMessages;

    private TimeSpan _start;

    private JsonTimelineMessage _timelineMessage;

    public IList<JsonTraceMessage> TraceMessages
    {
      get { return _traceMessages; }
    }

    public GlimpseTraceWriter(IMessageBroker messageBroker, Func<IExecutionTimer> timerStrategy)
      : this(messageBroker, timerStrategy, null)
    {
    }

    public GlimpseTraceWriter(IMessageBroker messageBroker, Func<IExecutionTimer> timerStrategy, ITraceWriter innerTraceWriter)
    {
      _messageBroker = messageBroker;
      _timerStrategy = timerStrategy;
      _innerTraceWriter = innerTraceWriter;
      _traceMessages = new List<JsonTraceMessage>();
    }

    public TraceLevel LevelFilter
    {
      get { return TraceLevel.Verbose; }
    }

    public void Trace(TraceLevel level, string message, Exception ex)
    {
      // write to any existing trace writer
      if (_innerTraceWriter != null && level <= _innerTraceWriter.LevelFilter)
        _innerTraceWriter.Trace(level, message, ex);

      IExecutionTimer timer = _timerStrategy();
      
      if (_traceMessages.Count > 0)
      {
        // check message to see if serialization is complete
        if (message.StartsWith("Serialized JSON:", StringComparison.Ordinal) || message.StartsWith("Deserialized JSON:", StringComparison.Ordinal))
        {
          TimerResult timeResult = null;
          if (timer != null)
          {
            timeResult = timer.Stop(_start);
            _timelineMessage.AsTimedMessage(timeResult);
          }

          // set final JSON onto previous message
          JsonTraceMessage lastMessage = _traceMessages.Last();
          lastMessage.JsonText = message.Substring(message.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Trim();
          lastMessage.Duration = (timeResult != null) ? (TimeSpan?) timeResult.Duration : null;

          _traceMessages.Clear();
          return;
        }
      }

      JsonAction action = JsonAction.Unknown;
      string type = null;
      string json = null;
      if (_traceMessages.Count == 0)
      {
        Match match = Regex.Match(message, @"^Started serializing ([^\s]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        if (match.Success)
        {
          type = match.Groups[1].Value.TrimEnd('.');
          action = JsonAction.Serialize;
        }
        else
        {
          match = Regex.Match(message, @"^Started deserializing ([^\s]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
          if (match.Success)
          {
            type = match.Groups[1].Value.TrimEnd('.');
            action = JsonAction.Deserialize;
          }
          else
          {
            if (message.StartsWith("Serialized JSON:", StringComparison.Ordinal))
              action = JsonAction.Serialize;
            else if (message.StartsWith("Deserialized JSON:", StringComparison.Ordinal))
              action = JsonAction.Deserialize;

            if (action != JsonAction.Unknown)
            {
              json = message.Substring(message.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Trim();
              message = null;
            }
          }
        }

        // create timeline message
        // will be updated each trace with new duration
        _timelineMessage = CreateJsonTimelineMessage(action, type);
        _messageBroker.Publish(_timelineMessage);

        if (timer != null)
          _start = timer.Start();
      }
      else
      {
        JsonTraceMessage previous = _traceMessages.Last();
        previous.Duration = null;

        action = previous.Action;
        type = previous.Type;
      }

      TimerResult result = null;
      if (timer != null)
      {
        result = timer.Stop(_start);
        _timelineMessage.AsTimedMessage(result);
      }

      JsonTraceMessage traceMessage = new JsonTraceMessage
        {
          Ordinal = _traceMessages.Count,
          MessageDate = DateTime.Now,
          Level = level,
          Message = message,
          Exception = ex,
          JsonText = json,
          Action = action,
          Type = (type != null) ? RemoveAssemblyDetails(type) : null,
          Duration = (result != null) ? (TimeSpan?)result.Duration : null
        };

      _messageBroker.Publish(traceMessage);
      _traceMessages.Add(traceMessage);
    }

    private static JsonTimelineMessage CreateJsonTimelineMessage(JsonAction action, string type)
    {
      JsonTimelineMessage timelineMessage = new JsonTimelineMessage();
      string eventName = action.ToString("G");
      if (type != null)
        eventName += " - " + RemoveAssemblyDetails(type);
      timelineMessage.AsTimelineMessage(eventName, new TimelineCategoryItem(action.ToString("G"), "#B3DF00", "#9BBB59"));
      return timelineMessage;
    }

    private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
    {
      // handle generics
      string[] parts = fullyQualifiedTypeName.Split('[');
      for (int i = 0; i < parts.Length; i++)
      {
        // handle multiple generic parameters
        string[] parameters = parts[i].Split(',');
        for (int j = 0; j < parameters.Length; j++)
        {
          parameters[j] = RemoveNamespaces(parameters[j]);
        }

        parts[i] = string.Join(",", parameters);
      }

      string fixedTypeName = string.Join("[", parts);
      return fixedTypeName;
    }

    private static string RemoveNamespaces(string text)
    {
      int typeStartIndex = text.LastIndexOf('.');
      if (typeStartIndex != -1)
        text = text.Substring(typeStartIndex + 1);

      return text;
    }
  }
}