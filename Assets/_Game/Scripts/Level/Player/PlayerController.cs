using Playtika.Controllers;
using Sling.Level.Collision;

namespace Sling.Level.Player
{
  public class PlayerController : ControllerBase
  {
    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    
    public PlayerController(IControllerFactory controllerFactory, PlayerView view, PlayerModel model) 
      : base(controllerFactory)
    {
      _view = view;
      _model = model;
    }

    protected override void OnStart()
    {
      Execute<IsInAirController, IsInAirController.Context>(
        new IsInAirController.Context(_model.IsInAir, _view.Config.GroundSurfaceLayerMask));
      
      Execute<PlayerLaunchController, PlayerLaunchController.Context>(
        new PlayerLaunchController.Context(_model.IsInAir));
    }
  }
}