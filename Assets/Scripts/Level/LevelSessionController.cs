using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class LevelSessionController : ControllerWithResultBase<LevelSessionResult>
{
    public LevelSessionController(IControllerFactory factory)
        : base(factory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
        IControllerFactory levelFactory = 
            await ExecuteAndWaitResultAsync<BuildLevelFactoryController, IControllerFactory>(ct);

        while (true)
        {
            GameplayOutcome outcome = 
                await ExecuteAndWaitResultAsync<LevelLoopController, GameplayOutcome>(levelFactory, ct);
        }
    }
}
