using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using PrimeTween;
using Sling.Common.UI;
using Sling.Infrastructure.Progress;
using Sling.Infrastructure.UI;
using Sling.Level.Hud;
using Sling.Level.Session;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private const string kNewBestScoreTitle = "NEW BEST SCORE!";

    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly HudView _hudView;
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly PlayerProgressService _playerProgressService;

    public LevelCompleteWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      HudView hudView,
      GameConfig gameConfig,
      GameModel gameModel,
      LevelModel levelModel,
      PlayerProgressService playerProgressService)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _hudView = hudView;
      _gameConfig = gameConfig;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _playerProgressService = playerProgressService;
    }

    private VisualElement _window;
    private VisualElement _root;

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.LevelCompleteWindowUxml, hasBackground: true);
      // The HUD overlay is a sibling of the window so it fades with the background instead of scaling.
      _root = _window.parent;

      // The window repeats the HUD panels on top of the blur; the level scene is reloaded afterwards.
      _hudView.SetVisible(false);

      LevelCompleteFlowResult result;
      Tween pulse = default;
      try
      {
        InitWindow();

        await _popUpWindowsRootView.ShowWindow(_window, ct);

        if (_levelModel.IsNewBestScore)
          pulse = StartNewBestScorePulse();

        result = await WaitForResult(ct);
        pulse.Stop();
        await _popUpWindowsRootView.HideWindow(_window, ct);
      }
      finally
      {
        pulse.Stop();
        _popUpWindowsRootView.RemoveWindow(_window);
      }

      Complete(result);
    }

    private void InitWindow()
    {
      SetCurrentRunInfo();
      SetBestResultInfo();
      SetNewBestScoreState();
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

    private void SetCurrentRunInfo()
    {
      _root.Q<Label>(WindowNames.LevelName).text = LevelScoreTextFormatter.FormatLevelName(_gameModel.LevelIndex);
      _root.Q<Label>(WindowNames.CurrentDeathCount).text =
        LevelScoreTextFormatter.FormatPlayerDeathCount(_levelModel.PlayerDeathCount.Value);
      _root.Q<Label>(WindowNames.CurrentElapsedTime).text =
        LevelScoreTextFormatter.FormatElapsedTime(_levelModel.ElapsedTimeInSeconds);
      _root.Q<Label>(WindowNames.CurrentLaunchCount).text =
        LevelScoreTextFormatter.FormatPlayerLaunchCount(_levelModel.PlayerLaunchCount.Value);
    }

    private void SetBestResultInfo()
    {
      if (!_playerProgressService.TryGetBestResult(_gameModel.SceneToLoad, out LevelBestResult bestResult))
        return;

      _window.Q<Label>(WindowNames.BestDeathCount).text =
        LevelScoreTextFormatter.FormatPlayerDeathCount(bestResult.DeathCount);
      _window.Q<Label>(WindowNames.BestElapsedTime).text =
        LevelScoreTextFormatter.FormatElapsedTime(bestResult.TimeInSeconds);
      _window.Q<Label>(WindowNames.BestLaunchCount).text =
        LevelScoreTextFormatter.FormatPlayerLaunchCount(bestResult.PlayerLaunchCount);
    }

    private void SetNewBestScoreState()
    {
      if (!_levelModel.IsNewBestScore)
        return;

      _window.Q<Label>(WindowNames.BestScoreTitle).text = kNewBestScoreTitle;
      _window.Q(WindowNames.BestScore).AddToClassList(WindowNames.Classes.NewBestScore);
      _window.Q(WindowNames.Crown).AddToClassList(WindowNames.Classes.NewBestScoreCrown);
    }

    private Tween StartNewBestScorePulse()
    {
      NewBestScorePulseConfig config = _gameConfig.NewBestScorePulse;
      if (config.Count <= 0)
        return default;

      VisualElement bestScore = _window.Q(WindowNames.BestScore);
      VisualElement crown = _window.Q(WindowNames.Crown);

      // One pulse is a grow + shrink, i.e. two yoyo cycles.
      return Tween.Custom(1f, config.Scale, config.Duration * 0.5f, value =>
        {
          var scale = new Scale(new Vector2(value, value));
          bestScore.style.scale = scale;
          crown.style.scale = scale;
        },
        config.Ease,
        cycles: config.Count * 2,
        cycleMode: CycleMode.Yoyo,
        startDelay: config.Delay);
    }
  }
}
