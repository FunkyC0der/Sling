using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Gameplay;
using Sling.Level.Player;
using Sling.Level.WinScreen;
using Sling.Utils;
using UnityEngine;
using VContainer.Unity;

namespace Sling.Level
{
  public class LevelSessionController : ControllerWithResultBase<LevelSessionResult>
  {
    public LevelSessionController(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      LifetimeScope levelScope = await ExecuteAndWaitResultAsync<BuildLevelScopeController, LifetimeScope>(ct);
      AddDisposable(levelScope);

      IControllerFactory levelControllerFactory = levelScope.GetControllerFactory();
      
      await ExecuteAndWaitResultAsync<SetPlayerStartPosController>(levelControllerFactory, ct);
      
      GameplayOutcome outcome;
      do
      {
        outcome =
          await ExecuteAndWaitResultAsync<GameplayLoopController, GameplayOutcome>(levelControllerFactory, ct);
        
        if(outcome == GameplayOutcome.Death)
          await ExecuteAndWaitResultAsync<RespawnPlayerController>(levelControllerFactory, ct);
        
      } while (outcome != GameplayOutcome.Win);

      await ExecuteAndWaitResultAsync<ShowWinScreenController, WinScreenResult>(levelControllerFactory, ct);
    }
  }
}