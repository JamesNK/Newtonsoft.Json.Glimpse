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
      IMessageBroker messageBroker = context.MessageBroker;
      Func<IExecutionTimer> timerStrategy = context.TimerStrategy;

      GlimpseJson.Initialize(messageBroker, timerStrategy);
    }
  }
}