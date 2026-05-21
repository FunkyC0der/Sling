using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Root.Game;
using Sling.Root.MainMenu.SelectLevel;

namespace Sling.Root.Flow
{
  public class MainMenuStateController : ControllerWithResultBase
  {
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;

    public MainMenuStateController(IControllerFactory controllerFactory, GameConfig gameConfig, GameModel gameModel)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
      _gameModel = gameModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await _gameConfig.MainMenuScene.LoadSceneAsync()
        .ToUniTask(cancellationToken: ct);

      int levelIndex = await ExecuteAndWaitResultAsync<SelectLevelWindowController, int>(ct);

      _gameModel.GameState = GameState.PlayLevels;
      _gameModel.LevelIndex = levelIndex;
      _gameModel.SceneToLoad = _gameConfig.LevelScenes[levelIndex].SceneName;

      Complete();
    }
  }
}
