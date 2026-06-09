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
      Rigidbody2D playerRb = _playerView.Rigidbody;
      
      float deceleration = Mathf.Abs(playerRb.linearVelocityX) / _playerView.Config.FinishStopDuration;

      while (Mathf.Abs(playerRb.linearVelocityX) > 0 && !cancellationToken.IsCancellationRequested)
      {
        playerRb.linearVelocityX =
          Mathf.MoveTowards(playerRb.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);

        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }

      playerRb.linearVelocityX = 0;
    }
  }
}
