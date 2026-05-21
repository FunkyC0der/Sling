using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.Gameplay
{
  public class RespawnPlayerController : ControllerWithResultBase<EmptyControllerResult>
  {
    private readonly PlayerView _playerView;
    private readonly LevelModel _levelModel;

    public RespawnPlayerController(IControllerFactory controllerFactory,
      PlayerView playerView,
      LevelModel levelModel)
      : base(controllerFactory)
    {
      _playerView = playerView;
      _levelModel = levelModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      await _playerView.Die();
      _playerView.Respawn(_levelModel.PlayerStartPos);
      Complete(new EmptyControllerResult());
    }
  }
}
