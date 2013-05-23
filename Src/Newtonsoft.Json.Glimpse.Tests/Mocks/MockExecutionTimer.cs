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
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;

namespace Newtonsoft.Json.Glimpse.Tests.Mocks
{
  public class MockExecutionTimer : IExecutionTimer
  {
    private int _seconds;

    public TimerResult Point()
    {
      throw new NotImplementedException();
    }

    public DateTime RequestStart
    {
      get { throw new NotImplementedException(); }
    }

    public TimeSpan Start()
    {
      return TimeSpan.FromSeconds(++_seconds);
    }

    public TimerResult Stop(TimeSpan offset)
    {
      return new TimerResult
        {
          StartTime = new DateTime(2000, 12, 12, 12, 12, 12, DateTimeKind.Utc),
          Duration = offset,
          Offset = offset
        };
    }

    public TimerResult Time(Action action)
    {
      throw new NotImplementedException();
    }

    public TimerResult<T> Time<T>(Func<T> function)
    {
      throw new NotImplementedException();
    }
  }
}