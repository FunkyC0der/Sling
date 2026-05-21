using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Root.Game;
using Sling.Root.MainMenu.SelectLevel;

namespace Sling.Root.Flow
{
  public class MainMenuStateController : ControllerWithResultBase
  {
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    private readonly MenuWindowsRootView _menuRootView;

    public MainMenuStateController(
      IControllerFactory controllerFactory,
      GameConfig gameConfig,
      GameModel gameModel,
      MenuWindowsRootView menuRootView)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
      _gameModel = gameModel;
      _menuRootView = menuRootView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await _gameConfig.MainMenuScene.LoadSceneAsync()
        .ToUniTask(cancellationToken: ct);

      int levelIndex = await ExecuteAndWaitResultAsync<SelectLevelWindowController, IWindowRootView, int>(
        _menuRootView, ct);

      _gameModel.GameState = GameState.PlayLevels;
      _gameModel.LevelIndex = levelIndex;
      _gameModel.SceneToLoad = _gameConfig.LevelScenes[levelIndex].SceneName;

      Complete();
    }
  }
}
