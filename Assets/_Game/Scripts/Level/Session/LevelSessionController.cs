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
    private LevelModel _levelModel;
    
    public LevelSessionController(IControllerFactory controllerFactory, LevelModel levelModel)
      : base(controllerFactory)
    {
      _levelModel = levelModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      await ExecuteAndWaitResultAsync<SavePlayerStartStatsFlowController>(cancellationToken);
      
      LevelSessionResult sessionResult;

      do
      {
        GameplayLoopResult loopResult =
          await ExecuteAndWaitResultAsync<GameplayLoopController, GameplayLoopResult>(cancellationToken);

        if (loopResult == GameplayLoopResult.Death)
        {
          await ExecuteAndWaitResultAsync<RespawnPlayerFlowController>(cancellationToken);
          continue;
        }

        if (loopResult == GameplayLoopResult.Win)
        {
          LevelCompleteFlowResult completeResult =
            await ExecuteAndWaitResultAsync<LevelCompleteFlowController, LevelCompleteFlowResult>(cancellationToken);

          sessionResult = ToSessionResult(completeResult);
          break;
        }

        sessionResult = loopResult switch
        {
          GameplayLoopResult.Restart => LevelSessionResult.Restart,
          GameplayLoopResult.Menu    => LevelSessionResult.Menu,
          _                          => LevelSessionResult.Next
        };

        break;
      } while (true);

      Complete(sessionResult);
    }

    private static LevelSessionResult ToSessionResult(LevelCompleteFlowResult result) =>
      result switch
      {
        LevelCompleteFlowResult.Next    => LevelSessionResult.Next,
        LevelCompleteFlowResult.Restart => LevelSessionResult.Restart,
        LevelCompleteFlowResult.Menu    => LevelSessionResult.Menu,
        _                               => throw new ArgumentOutOfRangeException(nameof(result), result, null)
      };
  }
}
