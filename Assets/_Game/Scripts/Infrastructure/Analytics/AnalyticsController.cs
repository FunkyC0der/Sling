using Playtika.Controllers;

namespace Sling.Infrastructure.Analytics
{
  public class AnalyticsController : ControllerBase
  {
    private readonly AnalyticsEvents _analyticsEvents;
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(
      IControllerFactory controllerFactory,
      AnalyticsEvents analyticsEvents,
      IAnalyticsService analyticsService)
      : base(controllerFactory)
    {
      _analyticsEvents = analyticsEvents;
      _analyticsService = analyticsService;
    }

    protected override void OnStart()
    {
      _analyticsEvents.RecordEvent += OnRecordEventRequested;
      AddDisposable(new DisposableToken(() =>
        _analyticsEvents.RecordEvent -= OnRecordEventRequested));
    }

    private void OnRecordEventRequested(Unity.Services.Analytics.Event analyticsEvent) =>
      _analyticsService.RecordEvent(analyticsEvent);
  }
}
