using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerFacingController : ControllerBase
  {
    private const float _kVelocityThreshold = 0.2f;
    
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;

    public PlayerFacingController(IControllerFactory controllerFactory, PlayerView view, LevelModel levelModel)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
    }

    protected override void OnStart()
    {
      _view.SetFacingLeft(_levelModel.PlayerIsFacingLeft);
      
      Update(CancellationToken).Forget();
    }

    private async UniTask Update(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (_view.LinearVelocityX > _kVelocityThreshold)
          _view.SetFacingLeft(false);
        else if (_view.LinearVelocityX < -_kVelocityThreshold)
          _view.SetFacingLeft(true);
        
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }
    }
  }
}
