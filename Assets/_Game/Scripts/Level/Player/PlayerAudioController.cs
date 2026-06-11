using Playtika.Controllers;
using Sling.Audio;

namespace Sling.Level.Player
{
  public class PlayerAudioController : ControllerBase
  {
    private readonly AudioEvents _audioEvents;
    private readonly PlayerModel _model;

    public PlayerAudioController(IControllerFactory controllerFactory, AudioEvents audioEvents, PlayerModel model)
      : base(controllerFactory)
    {
      _audioEvents = audioEvents;
      _model = model;
    }

    protected override void OnStart()
    {
      _model.OnPreLaunch += OnPreLaunch;
      _model.OnLaunched += OnLaunched;
      _model.IsGrounded.OnValueChanged += OnGroundedChanged;
    }

    private void OnPreLaunch() => 
      _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerStretch);

    private void OnLaunched() => 
      _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerLaunch);

    private void OnGroundedChanged(bool oldValue, bool newValue)
    {
      if(newValue)
        _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerLand);
    }
  }
}