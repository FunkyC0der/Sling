using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Gameplay;
using Sling.Level.Player;
using Sling.Level.WinScreen;

namespace Sling.Level
{
  public class LevelSessionController : ControllerWithResultBase
  {
    public LevelSessionController(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await ExecuteAndWaitResultAsync<SetPlayerStartPosController>(ct);
      
      GameplayOutcome outcome;
      do
      {
        outcome =
          await ExecuteAndWaitResultAsync<GameplayLoopController, GameplayOutcome>(ct);
        
        if(outcome == GameplayOutcome.Death)
          await ExecuteAndWaitResultAsync<RespawnPlayerController>(ct);
        
      } while (outcome != GameplayOutcome.Win);

      await ExecuteAndWaitResultAsync<FinishLevelFlowController, WinScreenResult>(ct);
      Complete();
    }
  }
}