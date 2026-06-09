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
      _view.SetFacingLeft(_levelModel.PlayerStartFlipX);
      
      Update(CancellationToken).Forget();
    }

    private async UniTask Update(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        _view.FaceByVelocityX(_kVelocityThreshold);
        
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }
    }
  }
}
