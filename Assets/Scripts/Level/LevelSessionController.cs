using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class LevelSessionController : ControllerWithResultBase<int, LevelSessionResult>
{
    public LevelSessionController(IControllerFactory factory) : base(factory) { }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        var context = await ExecuteAndWaitResultAsync<LoadLevelController, int, LevelSceneContext>(
            Args, cancellationToken);

        while (true)
        {
            var outcome = await ExecuteAndWaitResultAsync<LevelGameplayController, LevelSceneContext, GameplayOutcome>(
                context, cancellationToken);

            if (outcome == GameplayOutcome.Restart)
                continue;

            var nextAction = await ExecuteAndWaitResultAsync<LevelResultController, GameplayOutcome, NextAction>(
                outcome, cancellationToken);

            if (nextAction == NextAction.Restart)
                continue;

            if (nextAction == NextAction.Next)
            {
                Complete(LevelSessionResult.Next);
                return;
            }

            Complete(LevelSessionResult.Menu);
            return;
        }
    }
}
