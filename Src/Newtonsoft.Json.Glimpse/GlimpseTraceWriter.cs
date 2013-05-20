using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
      if (_innerTraceWriter != null && level >= _innerTraceWriter.LevelFilter)
        _innerTraceWriter.Trace(level, message, ex);

      // check message to see if serialization is complete
      if (_traceMessages.Count > 1)
      {
        if (message.StartsWith("Serialized JSON:", StringComparison.Ordinal) || message.StartsWith("Deserialized JSON:", StringComparison.Ordinal))
        {
          TimerResult timeResult = _timerStrategy().Stop(_start);
          _timelineMessage.AsTimedMessage(timeResult);

          // set final JSON onto previous message
          JsonTraceMessage lastMessage = _traceMessages.Last();
          lastMessage.JsonText = message.Substring(message.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Trim();
          lastMessage.Duration = timeResult.Duration;

          _traceMessages.Clear();
          return;
        }
      }

      JsonTraceMessage traceMessage = new JsonTraceMessage
        {
          Ordinal = _traceMessages.Count,
          MessageDate = DateTime.Now,
          Level = level,
          Message = message,
          Exception = ex
        };

      JsonAction action;
      string type;
      if (_traceMessages.Count == 0)
      {
        Match match = Regex.Match(traceMessage.Message, @"^Started serializing ([^\s]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        if (match.Success)
        {
          type = match.Groups[1].Value.TrimEnd('.');
          action = JsonAction.Serialize;
        }
        else
        {
          match = Regex.Match(traceMessage.Message, @"^Started deserializing ([^\s]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
          if (match.Success)
          {
            type = match.Groups[1].Value.TrimEnd('.');
            action = JsonAction.Deserialize;
          }
          else
          {
            type = null;
            action = JsonAction.Unknown;
          }
        }

        // create timeline message
        // will be updated each trace with new duration
        _timelineMessage = new JsonTimelineMessage();
        _timelineMessage.AsTimelineMessage(action.ToString("G") + " - " + RemoveAssemblyDetails(type), new TimelineCategoryItem(action.ToString("G"), "#B3DF00", "#9BBB59"));
        _messageBroker.Publish(_timelineMessage);

        _start = _timerStrategy().Start();
      }
      else
      {
        JsonTraceMessage previous = _traceMessages.Last();
        previous.Duration = null;

        action = previous.Action;
        type = previous.Type;
      }

      TimerResult result = _timerStrategy().Stop(_start);
      _timelineMessage.AsTimedMessage(result);

      traceMessage.Action = action;
      traceMessage.Type = RemoveAssemblyDetails(type);
      traceMessage.Duration = result.Duration;

      _messageBroker.Publish(traceMessage);
      _traceMessages.Add(traceMessage);
    }

    private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
    {
      // handle generics
      string[] parts = fullyQualifiedTypeName.Split('[');
      for (int i = 0; i < parts.Length; i++)
      {
        // handle multiple generic parameters
        string[] partsparts = parts[i].Split(',');
        for (int j = 0; j < partsparts.Length; j++)
        {
          partsparts[j] = RemoveNamespaces(partsparts[j]);
        }

        parts[i] = string.Join(",", partsparts);
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