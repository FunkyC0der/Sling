using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
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
    private readonly PlayerView _playerView;
    private readonly FinishZoneView _finishZoneView;
    private readonly AudioEvents _audioEvents;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly AnalyticsEvents _analyticsEvents;
    private readonly PlayerProgressService _playerProgressService;
    private readonly IPlayerAuthenticationService _playerAuthenticationService;
    private readonly ILeaderboardService _leaderboardService;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      PlayerView playerView,
      IOptionalViewProvider optionalViewProvider,
      AudioEvents audioEvents,
      GameModel gameModel,
      LevelModel levelModel,
      AnalyticsEvents analyticsEvents,
      PlayerProgressService playerProgressService,
      IPlayerAuthenticationService playerAuthenticationService,
      ILeaderboardService leaderboardService)
      : base(factory)
    {
      _playerView = playerView;
      _finishZoneView = optionalViewProvider.Get<FinishZoneView>();
      _audioEvents = audioEvents;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _analyticsEvents = analyticsEvents;
      _playerProgressService = playerProgressService;
      _playerAuthenticationService = playerAuthenticationService;
      _leaderboardService = leaderboardService;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _audioEvents.PlaySFX?.Invoke(AudioClipId.LevelComplete);
      
      _analyticsEvents.RecordEvent?.Invoke(new LevelCompletedEvent(_gameModel.LevelIndex,
        _levelModel.PlayerDeathCount.Value,
        _levelModel.ElapsedTimeInSeconds));

      LevelBestResult bestResult = SaveBestResultIfNeeded();

      if(_levelModel.IsNewBestScore)
        SubmitBestScoreIfSignedInAsync(bestResult, ct).Forget();
      
      await UniTask.WhenAll(
        OptionalFinishZoneBlinkAnim(),
        _playerView.StopHorizontalMovementAsync(_playerView.Config.FinishStopDuration, ct));
      
      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, LevelCompleteFlowResult>(ct);

      Complete(result);
    }

    private async UniTask OptionalFinishZoneBlinkAnim()
    {
      if (_finishZoneView)
        await _finishZoneView.Blink();
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
