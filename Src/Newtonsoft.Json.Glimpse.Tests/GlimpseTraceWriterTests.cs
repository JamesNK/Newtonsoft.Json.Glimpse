using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Glimpse.Messages;
using Newtonsoft.Json.Glimpse.Tests.Mocks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Glimpse.Tests
{
  [TestFixture]
  public class GlimpseTraceWriterTests
  {
    [Test]
    public void Wrapped()
    {
      MockMessageBroker broker = new MockMessageBroker();
      MockExecutionTimer timer = new MockExecutionTimer();

      MemoryTraceWriter innerWriter = new MemoryTraceWriter();

      GlimpseTraceWriter writer = new GlimpseTraceWriter(broker, () => timer, innerWriter);
      writer.Trace(TraceLevel.Verbose, "Random text", null);
      writer.Trace(TraceLevel.Error, "More random text", null);
      Assert.AreEqual(2, writer.TraceMessages.Count);

      Assert.AreEqual(2, innerWriter.GetTraceMessages().Count());

      Assert.AreEqual("Verbose Random text", innerWriter.GetTraceMessages().ElementAt(0).Substring(24));
      Assert.AreEqual("Error More random text", innerWriter.GetTraceMessages().ElementAt(1).Substring(24));

      innerWriter.LevelFilter = TraceLevel.Warning;
      writer.Trace(TraceLevel.Verbose, "Random text", null);
      writer.Trace(TraceLevel.Warning, "More random text", null);
      writer.Trace(TraceLevel.Error, "More random text", null);
      Assert.AreEqual(4, innerWriter.GetTraceMessages().Count());

      Assert.AreEqual(TraceLevel.Verbose, writer.LevelFilter);
    }

    [Test]
    public void Unknown()
    {
      MockMessageBroker broker = new MockMessageBroker();
      MockExecutionTimer timer = new MockExecutionTimer();

      GlimpseTraceWriter writer = new GlimpseTraceWriter(broker, () => timer);
      writer.Trace(TraceLevel.Verbose, "Random text", null);
      writer.Trace(TraceLevel.Verbose, "More random text", null);
      Assert.AreEqual(2, writer.TraceMessages.Count);

      IList<JsonTraceMessage> messages = broker.Messages.OfType<JsonTraceMessage>().ToList();
      Assert.AreEqual(2, messages.Count);

      Assert.AreEqual(JsonAction.Unknown, messages[0].Action);
      Assert.AreEqual(null, messages[0].Type);
      Assert.AreEqual(null, messages[0].Duration);
      Assert.AreEqual("Random text", messages[0].Message);

      Assert.AreEqual(JsonAction.Unknown, messages[1].Action);
      Assert.AreEqual(null, messages[1].Type);
      Assert.AreEqual(TimeSpan.FromSeconds(1), messages[1].Duration);
      Assert.AreEqual("More random text", messages[1].Message);
    }

    [Test]
    public void Serialize()
    {
      MockMessageBroker broker = new MockMessageBroker();
      MockExecutionTimer timer = new MockExecutionTimer();

      GlimpseTraceWriter writer = new GlimpseTraceWriter(broker, () => timer);
      writer.Trace(TraceLevel.Verbose, "Started serializing System.Int32.", null);
      writer.Trace(TraceLevel.Verbose, "Something something dark side.", null);
      Exception exception = null;
      try
      {
        exception = new Exception("Test exception.");
        throw exception;
      }
      catch (Exception ex)
      {
        writer.Trace(TraceLevel.Verbose, "Error!", ex);
      }
      Assert.AreEqual(3, writer.TraceMessages.Count);

      writer.Trace(TraceLevel.Verbose, "Serialized JSON:" + Environment.NewLine + new JArray(1, 2, 3), null);
      Assert.AreEqual(0, writer.TraceMessages.Count);

      IList<JsonTraceMessage> messages = broker.Messages.OfType<JsonTraceMessage>().ToList();
      Assert.AreEqual(3, messages.Count);

      Assert.AreEqual(JsonAction.Serialize, messages[0].Action);
      Assert.AreEqual("Int32", messages[0].Type);
      Assert.AreEqual(null, messages[0].Duration);
      Assert.AreEqual("Started serializing System.Int32.", messages[0].Message);

      Assert.AreEqual(JsonAction.Serialize, messages[1].Action);
      Assert.AreEqual("Int32", messages[1].Type);
      Assert.AreEqual(null, messages[1].Duration);
      Assert.AreEqual("Something something dark side.", messages[1].Message);

      Assert.AreEqual(JsonAction.Serialize, messages[2].Action);
      Assert.AreEqual("Int32", messages[2].Type);
      Assert.AreEqual(TimeSpan.FromSeconds(1), messages[2].Duration);
      Assert.AreEqual("Error!", messages[2].Message);
      Assert.AreEqual(exception, messages[2].Exception);
      Assert.AreEqual(@"[
  1,
  2,
  3
]", messages[2].JsonText);
    }

    [Test]
    public void Deserialize()
    {
      MockMessageBroker broker = new MockMessageBroker();
      MockExecutionTimer timer = new MockExecutionTimer();

      GlimpseTraceWriter writer = new GlimpseTraceWriter(broker, () => timer);
      writer.Trace(TraceLevel.Verbose, "Started deserializing System.Int32.", null);
      writer.Trace(TraceLevel.Verbose, "Something something dark side.", null);
      Exception exception = null;
      try
      {
        exception = new Exception("Test exception.");
        throw exception;
      }
      catch (Exception ex)
      {
        writer.Trace(TraceLevel.Verbose, "Error!", ex);
      }
      Assert.AreEqual(3, writer.TraceMessages.Count);

      writer.Trace(TraceLevel.Verbose, "Deserialized JSON:" + Environment.NewLine + new JArray(1, 2, 3), null);
      Assert.AreEqual(0, writer.TraceMessages.Count);

      IList<JsonTraceMessage> messages = broker.Messages.OfType<JsonTraceMessage>().ToList();
      Assert.AreEqual(3, messages.Count);

      Assert.AreEqual(JsonAction.Deserialize, messages[0].Action);
      Assert.AreEqual("Int32", messages[0].Type);
      Assert.AreEqual(null, messages[0].Duration);
      Assert.AreEqual("Started deserializing System.Int32.", messages[0].Message);

      Assert.AreEqual(JsonAction.Deserialize, messages[1].Action);
      Assert.AreEqual("Int32", messages[1].Type);
      Assert.AreEqual(null, messages[1].Duration);
      Assert.AreEqual("Something something dark side.", messages[1].Message);

      Assert.AreEqual(JsonAction.Deserialize, messages[2].Action);
      Assert.AreEqual("Int32", messages[2].Type);
      Assert.AreEqual(TimeSpan.FromSeconds(1), messages[2].Duration);
      Assert.AreEqual("Error!", messages[2].Message);
      Assert.AreEqual(exception, messages[2].Exception);
      Assert.AreEqual(@"[
  1,
  2,
  3
]", messages[2].JsonText);
    }
  }
}