using System;
using Glimpse.Core.Message;

namespace Newtonsoft.Json.Glimpse.Messages
{
  internal class JsonTimelineMessage : ITimelineMessage
  {
    public Guid Id { get; set; }
    public TimelineCategoryItem EventCategory { get; set; }
    public string EventName { get; set; }
    public string EventSubText { get; set; }
    public TimeSpan Duration { get; set; }
    public TimeSpan Offset { get; set; }
    public DateTime StartTime { get; set; }
  }
}