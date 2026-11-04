using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Player.States;

namespace Sling.Level.Player.InputGate
{
  /// <summary>The single owner of <see cref="PlayerInputView"/> enable/disable.</summary>
  public class PlayerInputGateController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerInputView _inputView;

    private bool? _enabled;

    public PlayerInputGateController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerInputView inputView)
      : base(controllerFactory)
    {
      _model = model;
      _inputView = inputView;
    }

    protected override void OnStart()
    {
      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);

      _model.IsPaused.OnValueChanged += OnPausedChanged;
      this.AddDisposableAction(() => _model.IsPaused.OnValueChanged -= OnPausedChanged);

      Refresh();
    }

    private void OnStateChanged(EPlayerState from, EPlayerState to) =>
      Refresh();

    private void OnPausedChanged(bool oldValue, bool newValue) =>
      Refresh();

    private void Refresh()
    {
      bool enabled = PlayerStates.AllowsLaunch(_model.State) && !_model.IsPaused.Value;
      if (_enabled == enabled)
        return;

      _enabled = enabled;
      if (enabled)
        _inputView.EnableInput();
      else
        _inputView.DisableInput();
    }
  }
}
