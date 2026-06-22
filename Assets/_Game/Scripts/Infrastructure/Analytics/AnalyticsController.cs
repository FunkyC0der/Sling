using Playtika.Controllers;
using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics
{
  public class AnalyticsController : ControllerBase
  {
    private readonly AnalyticsEvents _analyticsEvents;

    public AnalyticsController(IControllerFactory controllerFactory, AnalyticsEvents analyticsEvents) 
      : base(controllerFactory)
    {
      _analyticsEvents = analyticsEvents;
    }

    protected override void OnStart() => 
      _analyticsEvents.RecordEvent += OnRecordEventRequested;
    
    protected override void OnStop() =>
      _analyticsEvents.RecordEvent -= OnRecordEventRequested;

    private static void OnRecordEventRequested(Event ev) => 
      AnalyticsService.Instance.RecordEvent(ev);
  }
}