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
using System.Globalization;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace Newtonsoft.Json.Glimpse.Tab
{
  internal class ListOfJsonModelConverter : SerializationConverter<List<JsonModel>>
  {
    public override object Convert(List<JsonModel> models)
    {
      TabSection section = new TabSection(new[]
        {
          "Action",
          "Level",
          "Message",
          "Duration"
        });

      foreach (JsonModel model in models)
      {
        string message;
        if (!string.IsNullOrEmpty(model.JsonText))
          message = string.Format(CultureInfo.InvariantCulture, "!<div>{0}</div><div style='margin:10px'><code class='prettyprint glimpse-code' data-codeType='javascript'>{1}</code></div>!", model.Message, model.JsonText);
        else if (model.Exception != null)
          message = model.Message + Environment.NewLine + Environment.NewLine + model.Exception;
        else
          message = model.Message;

        section
          .AddRow()
          .Column(model.Action.ToString("G") + " " + model.Type)
          .Column((model.Level <= TraceLevel.Warning) ? string.Format(CultureInfo.InvariantCulture, "!<div style='color:red'>{0}</div>!", model.Level.ToString("G")) : model.Level.ToString("G"))
          .Column(message)
          .Column(model.Duration.HasValue ? model.Duration.Value.TotalMilliseconds.ToString("0.##", CultureInfo.InvariantCulture) + " ms" : null)
          .SelectedIf(model.Duration.HasValue);
      }

      return section.Build();
    }
  }
}