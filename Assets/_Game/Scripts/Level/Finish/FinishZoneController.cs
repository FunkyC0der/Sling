using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Finish
{
  public class FinishZoneController : ControllerBase
  {
    private readonly FinishZoneView _finishZoneView;
    private readonly LevelEvents _events;

    public FinishZoneController(IControllerFactory factory, FinishZoneView finishZoneView, LevelEvents events)
      : base(factory)
    {
      _finishZoneView = finishZoneView;
      _events = events;
    }

    protected override void OnStart() =>
      _finishZoneView.OnReached += _events.OnFinishReached;

    protected override void OnStop() =>
      _finishZoneView.OnReached -= _events.OnFinishReached;
  }
}
