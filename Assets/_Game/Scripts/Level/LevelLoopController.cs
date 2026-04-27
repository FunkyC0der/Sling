using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Player;

namespace Sling.Level
{
    public class LevelLoopController : ControllerWithResultBase<GameplayOutcome>
    {
        private readonly LevelEvents _events;

        public LevelLoopController(IControllerFactory factory, LevelEvents events)
            : base(factory)
        {
            _events = events;
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

            GameplayOutcome outcome = await outcomeSource.Task.AttachExternalCancellation(cancellationToken);
            Complete(outcome);
        }
    }
}
