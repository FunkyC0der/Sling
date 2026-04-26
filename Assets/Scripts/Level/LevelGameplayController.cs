using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class LevelGameplayController : ControllerWithResultBase<LevelSceneContext, GameplayOutcome>
{
    public LevelGameplayController(IControllerFactory factory) : base(factory) { }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        var events = Args.LevelEvents;
        var outcomeSource = new UniTaskCompletionSource<GameplayOutcome>();

        void OnDied() => outcomeSource.TrySetResult(GameplayOutcome.Death);
        void OnWon() => outcomeSource.TrySetResult(GameplayOutcome.Win);
        void OnRestart() => outcomeSource.TrySetResult(GameplayOutcome.Restart);

        events.OnPlayerDied += OnDied;
        events.OnFinishReached += OnWon;
        events.OnRestartRequested += OnRestart;
        AddDisposable(new DisposableToken(() =>
        {
            events.OnPlayerDied -= OnDied;
            events.OnFinishReached -= OnWon;
            events.OnRestartRequested -= OnRestart;
        }));

        Execute<PlayerController, LevelSceneContext>(Args);
        Execute<HazardsController, LevelSceneContext>(Args);
        Execute<FinishController, LevelSceneContext>(Args);
        Execute<HudController, LevelSceneContext>(Args);

        var outcome = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
        Complete(outcome);
    }
}
