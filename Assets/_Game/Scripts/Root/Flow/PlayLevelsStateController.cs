using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Session;
using Sling.Root.Game;
using Sling.Root.LevelLoading;
using UnityEngine;
using VContainer.Unity;

namespace Sling.Root.Flow
{
  public class PlayLevelsStateController : ControllerWithResultBase
  {
    private readonly GameModel _gameModel;
    private readonly GameConfig _gameConfig;

    public PlayLevelsStateController(IControllerFactory factory, GameModel gameModel, GameConfig gameConfig)
      : base(factory)
    {
      _gameModel = gameModel;
      _gameConfig = gameConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      do
      {
        await ExecuteAndWaitResultAsync<LoadLevelController, string>(_gameModel.SceneToLoad, ct);

        LifetimeScope levelScope = await ExecuteAndWaitResultAsync<BuildLevelScopeController, LifetimeScope>(ct);

        LevelSessionResult sessionResult =
          await ExecuteAndWaitResultAsync<LevelSessionController, LevelSessionResult>(levelScope.GetControllerFactory(), ct);

        levelScope.Dispose();

        if (sessionResult == LevelSessionResult.Menu)
        {
          _gameModel.GameState = GameState.MainMenu;
          break;
        }

        if (sessionResult == LevelSessionResult.Next)
        {
          _gameModel.LevelIndex = Mathf.Min(_gameModel.LevelIndex + 1, _gameConfig.LevelScenes.Count - 1);
          _gameModel.SceneToLoad = _gameConfig.LevelScenes[_gameModel.LevelIndex].SceneName;
        }
      } while (true);

      Complete();
    }
  }
}
