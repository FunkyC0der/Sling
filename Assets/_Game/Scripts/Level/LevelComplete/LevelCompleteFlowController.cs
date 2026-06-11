using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.UI.Windows;
using Sling.Level.Player;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly PlayerView _playerView;
    private readonly PopupWindowsRootView _popupRootView;
    private readonly AudioEvents _audioEvents;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      PlayerView playerView,
      PopupWindowsRootView popupRootView, 
      AudioEvents audioEvents)
      : base(factory)
    {
      _playerView = playerView;
      _popupRootView = popupRootView;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _audioEvents.PlaySFX?.Invoke(AudioClipId.LevelComplete);
      
      await _playerView.StopHorizontalMovementAsync(_playerView.Config.FinishStopDuration, cancellationToken);

      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, IWindowRootView, LevelCompleteFlowResult>(
          _popupRootView.NonSkippable(), cancellationToken);

      Complete(result);
    }
  }
}
