﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace Newtonsoft.Json.Glimpse.Tabs
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