using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.Extensions;
using Sling.Level.Player.States;
using Sling.Level.Session;

namespace Sling.Level.Player.Death
{
  /// <summary>Runs the death flow when the FSM enters <see cref="EPlayerState.Dead"/>.</summary>
  public class PlayerDeathController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerView _playerView;
    private readonly PlayerDeathView _deathView;
    private readonly LevelEvents _levelEvents;
    private readonly AudioEvents _audioEvents;

    public PlayerDeathController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerView playerView,
      PlayerDeathView deathView,
      LevelEvents levelEvents,
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _model = model;
      _playerView = playerView;
      _deathView = deathView;
      _levelEvents = levelEvents;
      _audioEvents = audioEvents;
    }

    protected override void OnStart()
    {
      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);
    }

    private void OnStateChanged(EPlayerState from, EPlayerState to)
    {
      if (to == EPlayerState.Dead)
        DeathFlowAsync().Forget();
    }

    private async UniTask DeathFlowAsync()
    {
      _playerView.FreezePhysics();
      _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerDeath);

      _levelEvents.OnPlayerDeathStarted?.Invoke();

      await _deathView.Die(CancellationToken);

      _levelEvents.OnPlayerDied?.Invoke();
    }
  }
}
