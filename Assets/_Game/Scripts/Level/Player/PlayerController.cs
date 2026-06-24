using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common;
using Sling.Level.Collision;
using Sling.Level.Gameplay;
using Sling.Level.Player.LandingDust;
using Sling.Level.Player.Launch;
using Sling.Level.Player.States;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class PlayerController : ControllerBase
  {
    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly LevelModel _levelModel;
    
    public PlayerController(IControllerFactory controllerFactory,
      PlayerView view,
      PlayerModel model,
      PlayerConfig config, 
      LevelModel levelModel) 
      : base(controllerFactory)
    {
      _view = view;
      _model = model;
      _config = config;
      _levelModel = levelModel;
    }

    protected override void OnStart() => 
      StartAsync().Forget();

    private async UniTask StartAsync()
    {
      if(_levelModel.PlayerDeathCount.Value > 0)
        await ExecuteAndWaitResultAsync<RespawnPlayerFlowController>(CancellationToken);
      
      Execute<PlayerStatesController>();
      Execute<IsWallSlidingController, Observable<bool>>(_model.IsWallSliding);
      
      Execute<IsInAirController, IsInAirController.Context>(
        new IsInAirController.Context(_model.IsInAir, _config.GroundSurfaceLayerMask));
      
      Execute<PlayerLaunchController>();
      Execute<PlayerDeathController>();
      
      Execute<PlayerAnimatorController>();
      Execute<PlayerAudioController>();
      Execute<PlayerLandingDustController>();
    }
  }
}
