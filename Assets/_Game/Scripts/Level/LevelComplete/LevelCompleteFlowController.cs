using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.Controllers;
using Sling.Common.UI.Windows;
using Sling.Common.Views;
using Sling.Infrastructure.Analytics;
using Sling.Infrastructure.Analytics.Events;
using Sling.Level.Finish;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly IViewsProvider _viewsProvider;
    private readonly AudioEvents _audioEvents;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly AnalyticsEvents _analyticsEvents;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      IViewsProvider viewsProvider,
      AudioEvents audioEvents,
      GameModel gameModel,
      LevelModel levelModel,
      AnalyticsEvents analyticsEvents)
      : base(factory)
    {
      _viewsProvider = viewsProvider;
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

      var playerView = _viewsProvider.Get<PlayerView>();
      
      await UniTask.WhenAll(
        OptionalFinishZoneBlinkAnim(),
        playerView.StopHorizontalMovementAsync(playerView.Config.FinishStopDuration, cancellationToken));


      var popupRootView = _viewsProvider.Get<PopupWindowsRootView>();
      
      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, IWindowRootView, LevelCompleteFlowResult>(
          popupRootView.NonSkippable(), cancellationToken);

      Complete(result);
    }

    private async UniTask OptionalFinishZoneBlinkAnim()
    {
      var finishZoneView = _viewsProvider.Get<FinishZoneView>();
      if (finishZoneView)
        await finishZoneView.Blink();
    }
  }
}
