using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Common.Views;
using Sling.Level.Boss;
using Sling.Level.Hud;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.Gameplay
{
  public class GameplayLoopController : ControllerWithResultBase<GameplayLoopResult>
  {
    private readonly LevelEvents _events;
    private readonly BossView _bossView;

    public GameplayLoopController(
      IControllerFactory factory,
      LevelEvents events,
      IOptionalViewProvider optionalViewProvider)
      : base(factory)
    {
      _events = events;
      _bossView = optionalViewProvider.Get<BossView>();
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      var outcomeSource = new UniTaskCompletionSource<GameplayLoopResult>();

      _events.OnPlayerDied       += OnDied;
      _events.OnLevelCompleted   += OnWon;
      _events.OnRestartRequested += OnRestart;
      _events.OnMenuRequested    += OnMenu;

      AddDisposable(new DisposableToken(() =>
      {
        _events.OnPlayerDied       -= OnDied;
        _events.OnLevelCompleted   -= OnWon;
        _events.OnRestartRequested -= OnRestart;
        _events.OnMenuRequested    -= OnMenu;
      }));

      Execute<PlayerScopeController>();
      Execute<HudController>();
      
      if (_bossView)
        ExecuteAndWaitResultAsync<BossFlowController>(cancellationToken).Forget();

      GameplayLoopResult loopResult = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
      Complete(loopResult);
      return;

      void OnDied()    => outcomeSource.TrySetResult(GameplayLoopResult.Death);
      void OnWon()     => outcomeSource.TrySetResult(GameplayLoopResult.Win);
      void OnRestart() => outcomeSource.TrySetResult(GameplayLoopResult.Restart);
      void OnMenu()    => outcomeSource.TrySetResult(GameplayLoopResult.Menu);
    }
  }
}
