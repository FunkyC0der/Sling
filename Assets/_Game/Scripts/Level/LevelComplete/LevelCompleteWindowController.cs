using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Common.UI;
using Sling.Common.UI.Windows;
using Sling.Infrastructure.Progress;
using Sling.Level.Session;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowController : WindowControllerBase<LevelCompleteFlowResult>
  {
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly PlayerProgressService _playerProgressService;

    public LevelCompleteWindowController(
      IControllerFactory factory,
      GameConfig gameConfig,
      GameModel gameModel,
      LevelModel levelModel,
      PlayerProgressService playerProgressService)
      : base(factory)
    {
      _gameConfig = gameConfig;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _playerProgressService = playerProgressService;
    }

    private VisualElement _window;
    private Label _playerDeathCountLabel;
    private Label _elapsedTimeLabel;
    private Label _newScoreLabel;
    
    protected override VisualTreeAsset WindowTemplate => _gameConfig.LevelCompleteWindowUxml;
    
    protected override void InitWindow(VisualElement window)
    {
      _window = window;
      _playerDeathCountLabel = _window.Q<Label>(WindowNames.kPlayerDeathCount);
      _elapsedTimeLabel = _window.Q<Label>(WindowNames.kElapsedTime);
      _newScoreLabel = _window.Q<Label>(WindowNames.NewScoreLabel);
      
      SetBestResultInfo();
      SetNewScoreLabelVisibility();
    }

    protected override UniTask<LevelCompleteFlowResult> WaitForResult(CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<LevelCompleteFlowResult>();
      
      _window.Q<Button>(WindowNames.NextLevelButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Next);
      
      _window.Q<Button>(WindowNames.RestartButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Restart);
      
      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(LevelCompleteFlowResult.Menu);
      
      return completionSource.Task.AttachExternalCancellation(cancellationToken);
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
