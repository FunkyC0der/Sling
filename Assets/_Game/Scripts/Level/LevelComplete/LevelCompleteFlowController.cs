using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player.Views;
using UnityEngine;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteFlowController : ControllerWithResultBase<LevelCompleteFlowResult>
  {
    private readonly PlayerView _playerView;
    private readonly GameConfig _gameConfig;
    
    public LevelCompleteFlowController(IControllerFactory factory, PlayerView playerView, GameConfig gameConfig)
      : base(factory)
    {
      _playerView = playerView;
      _gameConfig = gameConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await StopPlayerMovementX(ct);
      
      LevelCompleteWindowView levelCompleteWindowView = Object.Instantiate(_gameConfig.LevelCompleteWindowViewPrefab);
      AddDisposable(new DisposableToken(() => Object.Destroy(levelCompleteWindowView.gameObject)));
      
      levelCompleteWindowView.OnNextLevelClicked += () => Complete(LevelCompleteFlowResult.Next);
      levelCompleteWindowView.OnRestartClicked += () => Complete(LevelCompleteFlowResult.Restart);
      levelCompleteWindowView.OnMenuClicked += () => Complete(LevelCompleteFlowResult.Menu);

      await ct.WaitUntilCanceled();
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
      
      _playerView.LinearVelocityX = 0;
    }
  }
}
