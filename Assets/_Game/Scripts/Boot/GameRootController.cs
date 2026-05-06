using System;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Boot
{
  public class GameRootController : RootController
  {
    public GameRootController(IControllerFactory factory) : base(factory)
    {
    }

    protected override void OnStart()
    {
      base.OnStart();
      RunAsync(CancellationToken).Forget(ex =>
      {
        if(ex is not OperationCanceledException)
          Debug.LogException(ex);
      });
    }

    private async UniTask RunAsync(System.Threading.CancellationToken ct)
    {
      await ExecuteAndWaitResultAsync<BootstrapController>(ct);
      await ExecuteAndWaitResultAsync<LevelsLoopController>(ct);
    }
  }
}