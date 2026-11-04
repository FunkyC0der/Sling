using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.Extensions;
using Sling.Level.Player.States;

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
      _model.PreLaunch += OnPreLaunch;
      this.AddDisposableAction(() => _model.PreLaunch -= OnPreLaunch);

      _model.Launched += OnLaunched;
      this.AddDisposableAction(() => _model.Launched -= OnLaunched);

      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);
    }

    private void OnPreLaunch() =>
      _audioEvents.PlayConcurrentSFX?.Invoke(AudioClipId.PlayerStretch);

    private void OnLaunched() =>
      _audioEvents.PlayConcurrentSFX?.Invoke(AudioClipId.PlayerLaunch);

    private void OnStateChanged(EPlayerState from, EPlayerState to)
    {
      if (PlayerStates.IsLanding(from, to))
        _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerLand);
    }
  }
}
