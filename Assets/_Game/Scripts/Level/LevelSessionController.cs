using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
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
    }
  }
}