using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Finish;
using Sling.Level.Hazards;
using Sling.Level.Player;
using Sling.Level.StickyWall;

namespace Sling.Level.Gameplay
{
  public class GameplayLoopController : ControllerWithResultBase<GameplayOutcome>
  {
    private readonly LevelEvents _events;

    public GameplayLoopController(IControllerFactory factory, LevelEvents events)
      : base(factory)
    {
      _events = events;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      var outcomeSource = new UniTaskCompletionSource<GameplayOutcome>();

      _events.OnPlayerDied += OnDied;
      _events.OnFinishReached += OnWon;
      _events.OnRestartRequested += OnRestart;

      AddDisposable(new DisposableToken(() =>
      {
        _events.OnPlayerDied -= OnDied;
        _events.OnFinishReached -= OnWon;
        _events.OnRestartRequested -= OnRestart;
      }));

      Execute<FinishZoneController>();

      Execute<PlayerLaunchController>();
      Execute<StickyWallsController>();
      Execute<HazardZonesController>();

      GameplayOutcome outcome = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
      Complete(outcome);
      return;

      void OnDied() => 
        outcomeSource.TrySetResult(GameplayOutcome.Death);

      void OnWon() => 
        outcomeSource.TrySetResult(GameplayOutcome.Win);

      void OnRestart() => 
        outcomeSource.TrySetResult(GameplayOutcome.Restart);
    }
  }
}