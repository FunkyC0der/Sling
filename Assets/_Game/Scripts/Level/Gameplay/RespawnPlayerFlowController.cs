using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Gameplay
{
  public class RespawnPlayerFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _playerView;
    private readonly LevelModel _levelModel;

    public RespawnPlayerFlowController(IControllerFactory controllerFactory,
      PlayerView playerView,
      LevelModel levelModel)
      : base(controllerFactory)
    {
      _playerView = playerView;
      _levelModel = levelModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      Rigidbody2D playerRb = _playerView.Rigidbody;

      playerRb.bodyType = RigidbodyType2D.Static;
      
      await _playerView
        .GetComponent<PlayerAnimatorView>()
        .Die(_playerView.Config.DieDuration, _playerView.Config.DieFlickerCount)
        .AttachExternalCancellation(cancellationToken);
      
      playerRb.bodyType = RigidbodyType2D.Dynamic;
      playerRb.position = _levelModel.PlayerStartPos;
      
      Complete();
    }
  }
}
