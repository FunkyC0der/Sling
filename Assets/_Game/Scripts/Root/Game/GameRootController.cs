using System;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Root.Flow;
using UnityEngine;

namespace Sling.Root.Game
{
  public class GameRootController : RootController
  {
    private readonly GameModel _gameModel;

    public GameRootController(IControllerFactory factory, GameModel gameModel)
      : base(factory)
    {
      _gameModel = gameModel;
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
      await ExecuteAndWaitResultAsync<InitFirstSceneController>(ct);

      while (!ct.IsCancellationRequested)
      {
        if(_gameModel.GameState == GameState.MainMenu)
          await ExecuteAndWaitResultAsync<MainMenuStateController>(ct);
        else if (_gameModel.GameState == GameState.PlayLevels)
          await ExecuteAndWaitResultAsync<PlayLevelsStateController>(ct);
      }
    }
  }
}
