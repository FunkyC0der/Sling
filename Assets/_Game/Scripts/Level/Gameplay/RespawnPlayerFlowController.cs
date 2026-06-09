using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player;
using Sling.Level.Session;

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
      _playerView.FreezePhysics();
      
      await _playerView.PlayDeathAsync(cancellationToken);
      
      _playerView.UnfreezePhysics();
      _playerView.SetPosition(_levelModel.PlayerStartPos);
      
      Complete();
    }
  }
}
