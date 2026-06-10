using Playtika.Controllers;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.Gameplay
{
  public class SavePlayerStartStatsFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;

    public SavePlayerStartStatsFlowController(IControllerFactory controllerFactory,
      PlayerView view,
      LevelModel levelModel) 
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
    }

    protected override void OnStart()
    {
      _levelModel.PlayerStartPos = _view.Position;
      _levelModel.PlayerIsFacingLeft = _view.IsFacingLeft; 
      
      Complete();
    }
  }
}