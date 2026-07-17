using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.UI.Windows;
using Sling.Common.Views;
using Sling.Infrastructure.Analytics;
using Sling.Infrastructure.Analytics.Events;
using Sling.Infrastructure.Authentication;
using Sling.Infrastructure.Leaderboards;
using Sling.Infrastructure.Progress;
using Sling.Level.Finish;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly IViewsProvider _viewsProvider;
    private readonly AudioEvents _audioEvents;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly AnalyticsEvents _analyticsEvents;
    private readonly PlayerProgressService _playerProgressService;
    private readonly PlayerAuthenticationService _playerAuthenticationService;
    private readonly LeaderboardService _leaderboardService;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      IViewsProvider viewsProvider,
      AudioEvents audioEvents,
      GameModel gameModel,
      LevelModel levelModel,
      AnalyticsEvents analyticsEvents,
      PlayerProgressService playerProgressService,
      PlayerAuthenticationService playerAuthenticationService,
      LeaderboardService leaderboardService)
      : base(factory)
    {
      _viewsProvider = viewsProvider;
      _audioEvents = audioEvents;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _analyticsEvents = analyticsEvents;
      _playerProgressService = playerProgressService;
      _playerAuthenticationService = playerAuthenticationService;
      _leaderboardService = leaderboardService;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _audioEvents.PlaySFX?.Invoke(AudioClipId.LevelComplete);
      
      _analyticsEvents.RecordEvent?.Invoke(new LevelCompletedEvent(_gameModel.LevelIndex,
        _levelModel.PlayerDeathCount.Value,
        _levelModel.ElapsedTimeInSeconds));

      LevelBestResult bestResult = SaveBestResultIfNeeded();

      var playerView = _viewsProvider.Get<PlayerView>();
      
      await UniTask.WhenAll(
        SubmitBestScoreIfSignedInAsync(bestResult, cancellationToken),
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

    private LevelBestResult SaveBestResultIfNeeded()
    {
      var result = new LevelBestResult(_levelModel.PlayerDeathCount.Value, _levelModel.ElapsedTimeInSeconds);

      if (_playerProgressService.TryGetBestResult(_gameModel.SceneToLoad, out LevelBestResult currentResult) &&
          !LevelBestResultComparer.IsBetter(result, currentResult))
        return currentResult;

      _levelModel.IsNewBestScore = true;
      _playerProgressService.SetBestResult(_gameModel.SceneToLoad, result);
      _playerProgressService.Save();
      return result;
    }

    private async UniTask SubmitBestScoreIfSignedInAsync(
      LevelBestResult bestResult,
      CancellationToken cancellationToken)
    {
      if (!_playerAuthenticationService.IsSignedIn())
        return;

      try
      {
        await _leaderboardService.SetPlayerScoreAsync(
          _gameModel.SceneToLoad,
          bestResult.DeathCount,
          bestResult.TimeInSeconds,
          cancellationToken);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        throw;
      }
      catch (Exception exception)
      {
        Debug.LogException(exception);
      }
    }
  }
}
