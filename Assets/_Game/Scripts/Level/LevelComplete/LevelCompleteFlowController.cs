using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Level.Player;
using UnityEngine;

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
      await StopPlayerMovementX(cancellationToken);

      LevelCompleteFlowResult result =
        await ExecuteAndWaitResultAsync<LevelCompleteWindowController, IWindowRootView, LevelCompleteFlowResult>(
          _popupRootView.NonSkippable(), cancellationToken);

      Complete(result);
    }

    private async UniTask StopPlayerMovementX(CancellationToken cancellationToken)
    {
      float deceleration = Mathf.Abs(_playerView.LinearVelocityX) / _playerView.Config.FinishStopDuration;

      while (Mathf.Abs(_playerView.LinearVelocityX) > 0 && !cancellationToken.IsCancellationRequested)
      {
        _playerView.LinearVelocityX =
          Mathf.MoveTowards(_playerView.LinearVelocityX, 0, deceleration * Time.fixedDeltaTime);

        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }

      _playerView.LinearVelocityX = 0;
    }
  }
}
