using Playtika.Controllers;

namespace Sling.Level.Player
{
  public class PlayerAnimatorController : ControllerBase
  {
    private readonly PlayerAnimatorView _view;
    private readonly PlayerModel _model;

    public PlayerAnimatorController(IControllerFactory controllerFactory, PlayerAnimatorView view, PlayerModel model)
      : base(controllerFactory)
    {
      _view = view;
      _model = model;
    }

    protected override void OnStart()
    {
      _model.OnLaunched += () => _view.Jump();
      _model.IsInAir.OnValueChanged += IsInAirOnOnValueChanged;
    }

    private void IsInAirOnOnValueChanged(bool oldValue, bool newValue)
    {
      if(!newValue)
        _view.Land();
    }
  }
}