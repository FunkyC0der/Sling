using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Level.Boss;
using Sling.Level.Finish;
using Sling.Level.Hazards;
using Sling.Level.Hud;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.Gameplay
{
  public class GameplayLoopController : ControllerWithResultBase<GameplayLoopResult>
  {
    private readonly LevelEvents _events;

    public GameplayLoopController(IControllerFactory factory, LevelEvents events)
      : base(factory)
    {
      _events = events;
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

      
      Execute<PlayerLaunchController>();
      
      Execute<OptionalFeatureController<FinishZoneController, FinishZoneView>>();
      Execute<HazardZonesController>();
      Execute<OptionalFeatureController<BossController, BossView>>();
      Execute<HudController>();

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
