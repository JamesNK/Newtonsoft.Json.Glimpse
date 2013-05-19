#region License
// Copyright (c) 2007 James Newton-King
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
using NUnit.Framework;

namespace Newtonsoft.Json.Glimpse.Tests
{
  [TestFixture]
  public abstract class TestFixtureBase
  {
    [SetUp]
    protected void TestSetup()
    {
      //CultureInfo turkey = CultureInfo.CreateSpecificCulture("tr");
      //Thread.CurrentThread.CurrentCulture = turkey;
      //Thread.CurrentThread.CurrentUICulture = turkey;
    }
  }

  public static class ExceptionAssert
  {
    public static void Throws<TException>(string message, Action action)
        where TException : Exception
    {
      try
      {
        action();

        Assert.Fail("Exception of type {0} expected; got none exception", typeof(TException).Name);
      }
      catch (TException ex)
      {
        if (message != null)
          Assert.AreEqual(message, ex.Message, "Unexpected exception message." + Environment.NewLine + "Expected: " + message + Environment.NewLine + "Got: " + ex.Message + Environment.NewLine + Environment.NewLine + ex);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Exception of type {0} expected; got exception of type {1}.", typeof(TException).Name, ex.GetType().Name), ex);
      }
    }
  }
}
