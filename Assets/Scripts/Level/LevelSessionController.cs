using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class LevelSessionController : ControllerWithResultBase<int, LevelSessionResult>
{
    public LevelSessionController(IControllerFactory factory) : base(factory) { }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        var levelFactory = await ExecuteAndWaitResultAsync<LoadLevelController, int, IControllerFactory>(
            Args, cancellationToken);

        while (true)
        {
            var outcome = await ExecuteAndWaitResultAsync<LevelGameplayController, GameplayOutcome>(
                levelFactory, cancellationToken);

            if (outcome == GameplayOutcome.Restart)
                continue;

            var nextAction = await ExecuteAndWaitResultAsync<LevelResultController, GameplayOutcome, NextAction>(
                outcome, levelFactory, cancellationToken);

            if (nextAction == NextAction.Restart)
                continue;

            Complete(nextAction == NextAction.Next ? LevelSessionResult.Next : LevelSessionResult.Menu);
            return;
        }
    }
}
