using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;
using Newtonsoft.Json.Glimpse.Messages;

namespace Newtonsoft.Json.Glimpse.Tabs
{
  internal class JsonTab : TabBase, IDocumentation, ITabSetup, ITabLayout, IKey
  {
    private static readonly object Layout;

    static JsonTab()
    {
      Layout = TabLayout.Create().Row(delegate(TabLayoutRow r)
        {
          r.Cell(0).WidthInPercent(20).AsKey();
          r.Cell(1);
          r.Cell(2);
          r.Cell(3).WidthInPixels(100).Class("mono").AlignRight();
        }).Build();
    }

    public string DocumentationUri
    {
      get { return "http://james.newtonking.com/"; }
    }

    public void Setup(ITabSetupContext context)
    {
      context.PersistMessages<JsonTraceMessage>();
    }

    public string Key
    {
      get { return "Newtonsoft.Json.Glimpse"; }
    }

    public override object GetData(ITabContext context)
    {
      IList<JsonModel> data = context.GetMessages<JsonTraceMessage>().Select(m => new JsonModel(m)).ToList();

      return data;
    }

    public override string Name
    {
      get { return "JSON"; }
    }

    public object GetLayout()
    {
      return Layout;
    }
  }
}