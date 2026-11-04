using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Player.States;

namespace Sling.Level.Player.LandingDust
{
  public class PlayerLandingDustController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerLandingDustView _dustView;

    public PlayerLandingDustController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerLandingDustView dustView)
      : base(controllerFactory)
    {
      _model = model;
      _dustView = dustView;
    }

    protected override void OnStart()
    {
      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);
    }

    private void OnStateChanged(EPlayerState from, EPlayerState to)
    {
      if (PlayerStates.IsLanding(from, to))
        _dustView.Play();
    }
  }
}
