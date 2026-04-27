using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

public class GameRootController : RootController
{
    public GameRootController(IControllerFactory factory) : base(factory) { }

    protected override void OnStart()
    {
        base.OnStart();
        RunAsync(CancellationToken).Forget(ex => Debug.LogException(ex));
    }

    private async UniTask RunAsync(System.Threading.CancellationToken ct)
    {
        await ExecuteAndWaitResultAsync<BootstrapController>(ct);
        Execute<GameLoopController>();
    }
}
