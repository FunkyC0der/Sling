using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Collision;
using Sling.Level.Gameplay;
using Sling.Level.Player.Launch;
using Sling.Level.Player.States;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class PlayerController : ControllerBase
  {
    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly LevelModel _levelModel;
    
    public PlayerController(IControllerFactory controllerFactory,
      PlayerView view,
      PlayerModel model, 
      LevelModel levelModel) 
      : base(controllerFactory)
    {
      _view = view;
      _model = model;
      _levelModel = levelModel;
    }

    protected override void OnStart() => 
      StartAsync().Forget();

    private async UniTask StartAsync()
    {
      await ExecuteAndWaitResultAsync<RespawnPlayerFlowController>(CancellationToken);
      
      Execute<PlayerStatesController>();
      
      Execute<IsInAirController, IsInAirController.Context>(
        new IsInAirController.Context(_model.IsInAir, _view.Config.GroundSurfaceLayerMask));
      
      Execute<PlayerLaunchController>();
      Execute<PlayerFacingController>();
      Execute<PlayerDeathController>();
      
      Execute<PlayerAudioController>();
    }
  }
}
