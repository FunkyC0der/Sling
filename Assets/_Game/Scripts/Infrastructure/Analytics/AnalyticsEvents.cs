using System;
using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics
{
  public class AnalyticsEvents
  {
    public Action<Event> RecordEvent;
  }
}