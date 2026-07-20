using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

namespace Sling.Infrastructure.Analytics
{
  public class UnityAnalyticsService : IAnalyticsService
  {
    public void GrantConsent()
    {
      ConsentState consentState = EndUserConsent.GetConsentState();
      if (consentState.AnalyticsIntent == ConsentStatus.Granted)
        return;

      consentState.AnalyticsIntent = ConsentStatus.Granted;
      EndUserConsent.SetConsentState(consentState);
    }

    public void RecordEvent(Event analyticsEvent) =>
      AnalyticsService.Instance.RecordEvent(analyticsEvent);
  }
}
