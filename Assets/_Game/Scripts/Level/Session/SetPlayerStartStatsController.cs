using Playtika.Controllers;
using Sling.Level.Player;

namespace Sling.Level.Session
{
  public class SetPlayerStartStatsController : ControllerWithResultBase<EmptyControllerResult>
  {
    private readonly LevelModel _levelModel;
    private readonly PlayerView _playerView;

    public SetPlayerStartStatsController(IControllerFactory controllerFactory, LevelModel levelModel, PlayerView playerView) : base(controllerFactory)
    {
      _levelModel = levelModel;
      _playerView = playerView;
    }

    protected override void OnStart()
    {
      _levelModel.PlayerStartPos = _playerView.Position;
      _levelModel.PlayerStartFlipX = _playerView.IsFacingLeft;
      Complete(new EmptyControllerResult());
    }
  }
}
