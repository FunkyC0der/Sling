using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerFaceDirectionController : ControllerBase
  {
    private const float _kVelocityThreshold = 0.2f;
    
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;

    public PlayerFaceDirectionController(IControllerFactory controllerFactory, PlayerView view, LevelModel levelModel)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
    }

    protected override void OnStart()
    {
      _view.BodySprite.flipX = _levelModel.PlayerStartFlipX;
      
      Update(CancellationToken).Forget();
    }

    private async UniTask Update(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (_view.Rigidbody.linearVelocityX > _kVelocityThreshold)
          _view.BodySprite.flipX = false;
        else if (_view.Rigidbody.linearVelocityX < -_kVelocityThreshold)
          _view.BodySprite.flipX = true;
        
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }
    }
  }
}