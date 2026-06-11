using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Audio;
using Sling.Common.UI.Windows;
using Sling.MainMenu.SelectLevel;

namespace Sling.Flow
{
  public class MainMenuStateController : ControllerWithResultBase
  {
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    private readonly MenuWindowsRootView _menuRootView;
    private readonly AudioEvents _audioEvents;

    public MainMenuStateController(
      IControllerFactory controllerFactory,
      GameConfig gameConfig,
      GameModel gameModel,
      MenuWindowsRootView menuRootView, 
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
      _gameModel = gameModel;
      _menuRootView = menuRootView;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await _gameConfig.MainMenuScene.LoadSceneAsync()
        .ToUniTask(cancellationToken: ct);

      _audioEvents.PlayMusic?.Invoke(AudioClipId.MainMenuTrack);
      
      int levelIndex = await ExecuteAndWaitResultAsync<SelectLevelWindowController, IWindowRootView, int>(
        _menuRootView, ct);

      _gameModel.GameState = GameState.PlayLevels;
      _gameModel.LevelIndex = levelIndex;
      _gameModel.SceneToLoad = _gameConfig.Levels[levelIndex].Scene.SceneName;

      Complete();
    }
  }
}
