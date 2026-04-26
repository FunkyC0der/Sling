using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

public class GameLoopController : ControllerBase
{
    public GameLoopController(IControllerFactory factory) : base(factory) { }

    protected override void OnStart()
    {
        RunGameLoop(CancellationToken).Forget(ex => Debug.LogException(ex));
    }

    private async UniTask RunGameLoop(System.Threading.CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var levelId = await ExecuteAndWaitResultAsync<MainMenuController, int>(ct);

            LevelSessionResult result;
            do
            {
                result = await ExecuteAndWaitResultAsync<LevelSessionController, int, LevelSessionResult>(levelId, ct);
                if (result == LevelSessionResult.Next)
                    levelId++;
            }
            while (result == LevelSessionResult.Next);
        }
    }
}
