using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class LevelGameplayController : ControllerWithResultBase<GameplayOutcome>
{
    private readonly LevelEvents _events;
    private readonly List<MovingSawView> _movingSaws;

    public LevelGameplayController(IControllerFactory factory, LevelEvents events, List<MovingSawView> movingSaws) : base(factory)
    {
        _events = events;
        _movingSaws = movingSaws;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        var outcomeSource = new UniTaskCompletionSource<GameplayOutcome>();

        void OnDied() => outcomeSource.TrySetResult(GameplayOutcome.Death);
        void OnWon() => outcomeSource.TrySetResult(GameplayOutcome.Win);
        void OnRestart() => outcomeSource.TrySetResult(GameplayOutcome.Restart);

        _events.OnPlayerDied += OnDied;
        _events.OnFinishReached += OnWon;
        _events.OnRestartRequested += OnRestart;

        AddDisposable(new DisposableToken(() =>
        {
            _events.OnPlayerDied -= OnDied;
            _events.OnFinishReached -= OnWon;
            _events.OnRestartRequested -= OnRestart;
        }));

        Execute<PlayerController>();
        Execute<HazardsController>();
        Execute<FinishController>();
        Execute<HudController>();

        foreach (var saw in _movingSaws)
            Execute<MovingSawController, MovingSawView>(saw);

        var outcome = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
        Complete(outcome);
    }
}
