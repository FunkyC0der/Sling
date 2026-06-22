using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.UI.Windows;
using Sling.Infrastructure.Analytics;
using Sling.Infrastructure.Analytics.Events;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly PlayerView _playerView;
    private readonly PopupWindowsRootView _popupRootView;
    private readonly AudioEvents _audioEvents;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly AnalyticsEvents _analyticsEvents;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      PlayerView playerView,
      PopupWindowsRootView popupRootView, 
      AudioEvents audioEvents, 
      GameModel gameModel,
      LevelModel levelModel,
      AnalyticsEvents analyticsEvents)
      : base(factory)
    {
      _playerView = playerView;
      _popupRootView = popupRootView;
      _audioEvents = audioEvents;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _analyticsEvents = analyticsEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _audioEvents.PlaySFX?.Invoke(AudioClipId.LevelComplete);
      
      _analyticsEvents.RecordEvent?.Invoke(new LevelCompletedEvent(_gameModel.LevelIndex,
        _levelModel.PlayerDeathCount.Value,
        _levelModel.ElapsedTimeInSeconds));

      await _playerView.StopHorizontalMovementAsync(_playerView.Config.FinishStopDuration, cancellationToken);

      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, IWindowRootView, LevelCompleteFlowResult>(
          _popupRootView.NonSkippable(), cancellationToken);

      Complete(result);
    }
  }
}
