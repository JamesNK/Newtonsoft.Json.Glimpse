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
using NUnit.Framework;
using Newtonsoft.Json.Glimpse.Tests.Mocks;

namespace Newtonsoft.Json.Glimpse.Tests
{
  [TestFixture]
  public class GlimpseJsonTests
  {
    [Test]
    public void Initialize_NoDefaultSettings()
    {
      try
      {
        JsonConvert.DefaultSettings = null;

        MockMessageBroker broker = new MockMessageBroker();
        MockExecutionTimer timer = new MockExecutionTimer();

        GlimpseJson.Initialize(() => RuntimePolicy.On, () => timer, broker);

        Assert.IsNotNull(JsonConvert.DefaultSettings);

        JsonSerializerSettings settings = JsonConvert.DefaultSettings();

        Assert.IsNotNull(settings.TraceWriter);
        Assert.IsInstanceOf<GlimpseTraceWriter>(settings.TraceWriter);
      }
      finally
      {
        JsonConvert.DefaultSettings = null;
      }
    }

    [Test]
    public void Initialize_DefaultSettings()
    {
      try
      {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
          Formatting = Formatting.Indented
        };

        MockMessageBroker broker = new MockMessageBroker();
        MockExecutionTimer timer = new MockExecutionTimer();

        GlimpseJson.Initialize(() => RuntimePolicy.PersistResults, () => timer, broker);

        Assert.IsNotNull(JsonConvert.DefaultSettings);

        JsonSerializerSettings settings = JsonConvert.DefaultSettings();

        Assert.IsNotNull(settings.TraceWriter);
        Assert.IsInstanceOf<GlimpseTraceWriter>(settings.TraceWriter);

        Assert.AreEqual(Formatting.Indented, settings.Formatting);
      }
      finally
      {
        JsonConvert.DefaultSettings = null;
      }
    }

    [Test]
    public void Initialize_GlimpseOff()
    {
      try
      {
        JsonConvert.DefaultSettings = null;

        MockMessageBroker broker = new MockMessageBroker();
        MockExecutionTimer timer = new MockExecutionTimer();

        GlimpseJson.Initialize(() => RuntimePolicy.Off, () => timer, broker);

        Assert.IsNotNull(JsonConvert.DefaultSettings);

        JsonSerializerSettings settings = JsonConvert.DefaultSettings();

        Assert.IsNull(settings.TraceWriter);
      }
      finally
      {
        JsonConvert.DefaultSettings = null;
      }
    }

    [Test]
    public void Initialize_DefaultSettings_GlimpseOff()
    {
      try
      {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
          Formatting = Formatting.Indented
        };

        MockMessageBroker broker = new MockMessageBroker();
        MockExecutionTimer timer = new MockExecutionTimer();

        GlimpseJson.Initialize(() => RuntimePolicy.ExecuteResourceOnly, () => timer, broker);

        Assert.IsNotNull(JsonConvert.DefaultSettings);

        JsonSerializerSettings settings = JsonConvert.DefaultSettings();

        Assert.IsNull(settings.TraceWriter);

        Assert.AreEqual(Formatting.Indented, settings.Formatting);
      }
      finally
      {
        JsonConvert.DefaultSettings = null;
      }
    }
  }
}
