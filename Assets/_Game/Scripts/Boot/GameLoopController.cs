using System;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level;
using UnityEngine;

namespace Sling.Boot
{
  public class GameLoopController : ControllerBase
  {
    public GameLoopController(IControllerFactory factory) : base(factory)
    {
    }

    protected override void OnStart()
    {
      RunGameLoop(CancellationToken).Forget(ex =>
      {
        if (ex is not OperationCanceledException)
          Debug.LogException(ex);
      });
    }

    private async UniTask RunGameLoop(System.Threading.CancellationToken ct)
    {
      while (!ct.IsCancellationRequested)
      {
        LevelSessionResult result;
        do
        {
          result = await ExecuteAndWaitResultAsync<LevelSessionController, LevelSessionResult>(ct);
          if (result == LevelSessionResult.Next)
            Debug.Log("Next level");
        } while (result == LevelSessionResult.Next);
      }
    }
  }
}