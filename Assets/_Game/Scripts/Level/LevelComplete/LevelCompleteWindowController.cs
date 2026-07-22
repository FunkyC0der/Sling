using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI;
using Sling.Infrastructure.Progress;
using Sling.Infrastructure.UI;
using Sling.Level.Session;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly PlayerProgressService _playerProgressService;

    public LevelCompleteWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig,
      GameModel gameModel,
      LevelModel levelModel,
      PlayerProgressService playerProgressService)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _playerProgressService = playerProgressService;
    }

    private VisualElement _window;
    private Label _playerDeathCountLabel;
    private Label _elapsedTimeLabel;
    private Label _newScoreLabel;

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.LevelCompleteWindowUxml, hasBackground: true);

      LevelCompleteFlowResult result;
      try
      {
        InitWindow();

        await _popUpWindowsRootView.ShowWindow(_window, ct);

        result = await WaitForResult(ct);
        await _popUpWindowsRootView.HideWindow(_window, ct);
      }
      finally
      {
        _popUpWindowsRootView.RemoveWindow(_window);
      }

      Complete(result);
    }

    private void InitWindow()
    {
      _playerDeathCountLabel = _window.Q<Label>(WindowNames.kPlayerDeathCount);
      _elapsedTimeLabel = _window.Q<Label>(WindowNames.kElapsedTime);
      _newScoreLabel = _window.Q<Label>(WindowNames.NewScoreLabel);
      
      SetBestResultInfo();
      SetNewScoreLabelVisibility();
    }

    private UniTask<LevelCompleteFlowResult> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<LevelCompleteFlowResult>();
      
      _window.Q<Button>(WindowNames.NextLevelButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Next);
      
      _window.Q<Button>(WindowNames.RestartButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Restart);
      
      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(LevelCompleteFlowResult.Menu);
      
      return completionSource.Task.AttachExternalCancellation(ct);
    }

    private void SetBestResultInfo()
    {
      if (!_playerProgressService.TryGetBestResult(_gameModel.SceneToLoad, out LevelBestResult bestResult))
        return;

      _playerDeathCountLabel.text = LevelScoreTextFormatter.FormatPlayerDeathCount(bestResult.DeathCount);
      _elapsedTimeLabel.text = LevelScoreTextFormatter.FormatElapsedTime(bestResult.TimeInSeconds);
    }

    private void SetNewScoreLabelVisibility() =>
      _newScoreLabel.style.display = _levelModel.IsNewBestScore ? DisplayStyle.Flex : DisplayStyle.None;
  }
}
