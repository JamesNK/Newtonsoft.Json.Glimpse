//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Glimpse.Core.Setting;

//namespace Newtonsoft.Json.Glimpse
//{
//  public static class Initialize
//  {
//    public static void Json(this Initializer initializer)
//    {
//      Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
//      if (defaultSettings != null)
//      {
//        // ensure any existing trace writer is wrapped
//        JsonConvert.DefaultSettings = () =>
//          {
//            JsonSerializerSettings existingSettings = defaultSettings() ?? new JsonSerializerSettings();
//            existingSettings.TraceWriter = new GlimpseTraceWriter(existingSettings.TraceWriter);

//            return existingSettings;
//          };
//      }
//      else
//      {
//        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
//          {
//            TraceWriter = new GlimpseTraceWriter()
//          };
//      }
//    }
//  }
//}