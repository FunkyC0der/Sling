using Playtika.Controllers;

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

    protected override void OnStart() => 
      _model.IsGrounded.OnValueChanged += OnGroundedChanged;

    private void OnGroundedChanged(bool oldValue, bool newValue)
    {
      if (!newValue)
        return;

      _dustView.Play();
    }
  }
}
