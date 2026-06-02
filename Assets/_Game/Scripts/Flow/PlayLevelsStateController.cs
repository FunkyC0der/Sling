using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Level;
using Sling.Level.Session;
using Sling.LevelLoading;
using UnityEngine;

namespace Sling.Flow
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

        LevelSessionResult sessionResult =
          await ExecuteAndWaitResultAsync<LevelScopeController, LevelSessionResult>(ct);

        if (sessionResult == LevelSessionResult.Menu)
        {
          _gameModel.GameState = GameState.MainMenu;
          break;
        }

        if (sessionResult == LevelSessionResult.Next)
        {
          _gameModel.LevelIndex = Mathf.Min(_gameModel.LevelIndex + 1, _gameConfig.Levels.Count - 1);
          _gameModel.SceneToLoad = _gameConfig.Levels[_gameModel.LevelIndex].Scene.SceneName;
        }
      } while (true);

      Complete();
    }
  }
}
