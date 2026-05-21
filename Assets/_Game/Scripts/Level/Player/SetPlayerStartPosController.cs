using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class SetPlayerStartPosController : ControllerWithResultBase<EmptyControllerResult>
  {
    private readonly LevelModel _levelModel;
    private readonly PlayerView _playerView;

    public SetPlayerStartPosController(IControllerFactory controllerFactory, LevelModel levelModel, PlayerView playerView) : base(controllerFactory)
    {
      _levelModel = levelModel;
      _playerView = playerView;
    }

    protected override void OnStart()
    {
      _levelModel.PlayerStartPos = _playerView.Position;
      Complete(new EmptyControllerResult());
    }
  }
}
