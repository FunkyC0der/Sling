using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics
{
  public class DummyAnalyticsService : IAnalyticsService
  {
    public void GrantConsent()
    {
    }

    public void RecordEvent(Event analyticsEvent)
    {
    }
  }
}
