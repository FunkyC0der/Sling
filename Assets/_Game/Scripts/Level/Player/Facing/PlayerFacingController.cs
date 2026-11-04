using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Player.States;

namespace Sling.Level.Player.Facing
{
  /// <summary>Flips the body sprite towards horizontal velocity while moving freely.</summary>
  public class PlayerFacingController : ControllerBase
  {
    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly UpdateEvents _updateEvents;

    public PlayerFacingController(IControllerFactory controllerFactory,
      PlayerView view,
      PlayerModel model,
      PlayerConfig config,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _view = view;
      _model = model;
      _config = config;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnFixedUpdate += OnFixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= OnFixedUpdate);
    }

    private void OnFixedUpdate()
    {
      if (_model.State is not (EPlayerState.Grounded or EPlayerState.Launching or EPlayerState.Airborne))
        return;

      float threshold = _config.FacingVelocityThreshold;
      if (_view.LinearVelocityX > threshold)
        _view.SetFacingLeft(false);
      else if (_view.LinearVelocityX < -threshold)
        _view.SetFacingLeft(true);
    }
  }
}
