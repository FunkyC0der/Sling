using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player.Views;
using Sling.Level.WinScreen;
using UnityEngine;

namespace Sling.Level.Gameplay
{
  public class FinishLevelFlowController : ControllerWithResultBase<WinScreenResult>
  {
    private readonly WinScreenView _winScreenView;
    private readonly PlayerView _playerView;

    public FinishLevelFlowController(IControllerFactory factory, WinScreenView winScreenView, PlayerView playerView)
      : base(factory)
    {
      _winScreenView = winScreenView;
      _playerView = playerView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await StopPlayerMovementX(ct);
      
      _playerView.LinearVelocityX = 0;
      
      _winScreenView.OnRestartClicked += OnRestartClicked;
      AddDisposable(new DisposableToken(() => _winScreenView.OnRestartClicked -= OnRestartClicked));
      
      _winScreenView.Show();
    }
    
    private async UniTask StopPlayerMovementX(CancellationToken ct)
    {
      float deceleration = Mathf.Abs(_playerView.LinearVelocityX) / _playerView.Config.FinishStopDuration;
      
      while (Mathf.Abs(_playerView.LinearVelocityX) > 0 && !ct.IsCancellationRequested)
      {
        _playerView.LinearVelocityX =
          Mathf.MoveTowards(_playerView.LinearVelocityX, 0, deceleration * Time.fixedDeltaTime);
        
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
      }
    }

    private static bool IsBetween(float value, float a, float b) =>
      value >= Mathf.Min(a, b) && value <= Mathf.Max(a, b);

    private void OnRestartClicked() => 
      Complete(WinScreenResult.Restart);
  }
}
