using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Level.Hazards;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class PlayerDeathController : ControllerBase
  {
    private readonly DamageableView _damageableView;
    private readonly PlayerView _playerView;
    private readonly PlayerAnimationsView _animationsView;
    private readonly PlayerInputView _inputView;
    private readonly LevelEvents _levelEvents;
    private readonly AudioEvents _audioEvents;
    private readonly PlayerModel _model;

    public PlayerDeathController(IControllerFactory controllerFactory,
      DamageableView damageableView,
      PlayerView playerView,
      PlayerAnimationsView animationsView,
      PlayerInputView inputView,
      LevelEvents levelEvents, 
      AudioEvents audioEvents, 
      PlayerModel model) 
      : base(controllerFactory)
    {
      _damageableView = damageableView;
      _playerView = playerView;
      _animationsView = animationsView;
      _inputView = inputView;
      _levelEvents = levelEvents;
      _audioEvents = audioEvents;
      _model = model;
    }

    protected override void OnStart() => 
      _damageableView.OnDamaged += OnDamaged;

    protected override void OnStop() => 
      _damageableView.OnDamaged -= OnDamaged;

    private void OnDamaged() => 
      DeathFlowAsync().Forget();

    private async UniTask DeathFlowAsync()
    {
      _damageableView.OnDamaged -= OnDamaged;

      _model.IsDead.Value = true;
      
      _playerView.FreezePhysics();
      _inputView.DisableInput();

      _audioEvents.PlaySFX(AudioClipId.PlayerDeath);
      
      await _animationsView.Die(CancellationToken);
      
      _levelEvents.OnPlayerDied?.Invoke();
    }
  }
}