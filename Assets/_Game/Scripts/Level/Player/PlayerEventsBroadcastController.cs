using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class PlayerEventsBroadcastController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly LevelEvents _levelEvents;

    public PlayerEventsBroadcastController(IControllerFactory controllerFactory, PlayerModel model, LevelEvents levelEvents) : base(controllerFactory)
    {
      _model = model;
      _levelEvents = levelEvents;
    }

    protected override void OnStart() => 
      _model.OnLaunched += ForwardLaunchEvent;

    private void ForwardLaunchEvent() =>
      _levelEvents.OnPlayerLaunched?.Invoke();
  }
}