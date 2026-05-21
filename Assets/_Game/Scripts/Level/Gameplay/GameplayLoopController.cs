using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Finish;
using Sling.Level.Hazards;
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
      _events.OnFinishReached    += OnWon;
      _events.OnRestartRequested += OnRestart;

      AddDisposable(new DisposableToken(() =>
      {
        _events.OnPlayerDied       -= OnDied;
        _events.OnFinishReached    -= OnWon;
        _events.OnRestartRequested -= OnRestart;
      }));

      Execute<FinishZoneController>();
      Execute<PlayerLaunchController>();
      Execute<HazardZonesController>();

      GameplayLoopResult loopResult = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
      Complete(loopResult);
      return;

      void OnDied()    => outcomeSource.TrySetResult(GameplayLoopResult.Death);
      void OnWon()     => outcomeSource.TrySetResult(GameplayLoopResult.Win);
      void OnRestart() => outcomeSource.TrySetResult(GameplayLoopResult.Restart);
    }
  }
}
