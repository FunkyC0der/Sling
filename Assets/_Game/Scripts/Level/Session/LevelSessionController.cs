using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Finish;
using Sling.Level.Gameplay;
using Sling.Level.LevelComplete;
using Sling.Level.Player;

namespace Sling.Level.Session
{
  public class LevelSessionController : ControllerWithResultBase<LevelSessionResult>
  {
    public LevelSessionController(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await ExecuteAndWaitResultAsync<SetPlayerStartPosController>(ct);

      LevelSessionResult sessionResult;

      do
      {
        GameplayLoopResult loopResult =
          await ExecuteAndWaitResultAsync<GameplayLoopController, GameplayLoopResult>(ct);

        if(loopResult == GameplayLoopResult.Death)
        {
          await ExecuteAndWaitResultAsync<RespawnPlayerController>(ct);
          continue;
        }

        if (loopResult == GameplayLoopResult.Menu)
        {
          sessionResult = LevelSessionResult.Menu;
          break;
        }

        if (loopResult == GameplayLoopResult.Restart)
        {
          sessionResult = LevelSessionResult.Restart;
          break;
        }

        if (loopResult == GameplayLoopResult.Win)
        {
          LevelCompleteFlowResult levelCompleteResult =
            await ExecuteAndWaitResultAsync<LevelCompleteFlowController, LevelCompleteFlowResult>(ct);

          sessionResult = ToSessionResult(levelCompleteResult);
          break;
        }
      } while (true);

      Complete(sessionResult);
    }

    private static LevelSessionResult ToSessionResult(LevelCompleteFlowResult levelCompleteResult)
    {
      return levelCompleteResult switch
      {
        LevelCompleteFlowResult.Restart => LevelSessionResult.Restart,
        LevelCompleteFlowResult.Next => LevelSessionResult.Next,
        LevelCompleteFlowResult.Menu => LevelSessionResult.Menu,
        _ => throw new ArgumentException($"Wrong argument {levelCompleteResult}")
      };
    }
  }
}
