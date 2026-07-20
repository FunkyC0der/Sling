using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics
{
  public interface IAnalyticsService
  {
    void GrantConsent();
    void RecordEvent(Event analyticsEvent);
  }
}
