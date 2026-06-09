using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Level.Player;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly PlayerView _playerView;
    private readonly PopupWindowsRootView _popupRootView;

    public LevelCompleteFlowController(
      IControllerFactory factory,
      PlayerView playerView,
      PopupWindowsRootView popupRootView)
      : base(factory)
    {
      _playerView = playerView;
      _popupRootView = popupRootView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      await _playerView.StopHorizontalMovementAsync(_playerView.Config.FinishStopDuration, cancellationToken);

      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, IWindowRootView, LevelCompleteFlowResult>(
          _popupRootView.NonSkippable(), cancellationToken);

      Complete(result);
    }
  }
}
