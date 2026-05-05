using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Gameplay;
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

      GameplayOutcome outcome =
        await ExecuteAndWaitResultAsync<GameplayLoopController, GameplayOutcome>(levelControllerFactory, ct);

      Debug.Log($"{outcome}");
      
      if (outcome == GameplayOutcome.Win)
      {
        WinScreenResult result =
          await ExecuteAndWaitResultAsync<WinScreenController, WinScreenResult>(levelControllerFactory, ct);
        
        Debug.Log($"{result}");
      }
      else if (outcome == GameplayOutcome.Death)
      {
        await ExecuteAndWaitResultAsync<GameOverController>(levelControllerFactory, ct);
      }
    }
  }
}