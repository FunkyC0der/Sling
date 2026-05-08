using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Finish;
using Sling.Level.Player.Views;
using UnityEngine;

namespace Sling.Level.WinScreen
{
  public class FinishLevelFlowController : ControllerWithResultBase<WinScreenResult>
  {
    private readonly WinScreenView _winScreenView;
    private readonly PlayerView _playerView;
    private readonly FinishZoneView _finishZoneView;

    public FinishLevelFlowController(IControllerFactory factory,
      WinScreenView winScreenView,
      PlayerView playerView,
      FinishZoneView finishZoneView)
      : base(factory)
    {
      _winScreenView = winScreenView;
      _playerView = playerView;
      _finishZoneView = finishZoneView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await WaitPlayerReachFinishZone(ct);
      
      _playerView.LinearVelocityX = 0;
      
      _winScreenView.OnRestartClicked += OnRestartClicked;
      AddDisposable(new DisposableToken(() => _winScreenView.OnRestartClicked -= OnRestartClicked));
      
      _winScreenView.Show();
    }

    private async UniTask WaitPlayerReachFinishZone(CancellationToken ct)
    {
      float prevPlayerPositionX = _playerView.transform.position.x;
      
      while (!ct.IsCancellationRequested)
      {
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
        
        if(IsBetween(_finishZoneView.transform.position.x, prevPlayerPositionX, _playerView.transform.position.x))
          break;
        
        prevPlayerPositionX = _playerView.transform.position.x;
      }
    }

    private static bool IsBetween(float value, float a, float b) =>
      value >= Mathf.Min(a, b) && value <= Mathf.Max(a, b);

    private void OnRestartClicked() => 
      Complete(WinScreenResult.Restart);
  }
}
