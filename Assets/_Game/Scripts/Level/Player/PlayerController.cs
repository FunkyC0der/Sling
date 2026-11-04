using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Common.OffScreenMarker;
using Sling.Level.Player.Animations;
using Sling.Level.Player.Death;
using Sling.Level.Player.Facing;
using Sling.Level.Player.InputGate;
using Sling.Level.Player.LandingDust;
using Sling.Level.Player.Launch;
using Sling.Level.Player.Sensors;
using Sling.Level.Player.States;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class PlayerController : ControllerBase
  {
    private readonly LevelModel _levelModel;

    public PlayerController(IControllerFactory controllerFactory, LevelModel levelModel)
      : base(controllerFactory)
    {
      _levelModel = levelModel;
    }

    protected override void OnStart() =>
      StartAsync().Forget();

    private async UniTask StartAsync()
    {
      if (_levelModel.PlayerDeathCount.Value > 0)
        await ExecuteAndWaitResultAsync<RespawnPlayerFlowController>(CancellationToken);

      // Sensors: write facts.
      Execute<PlayerGroundSensorController>();
      Execute<PlayerSurfaceContactSensorController>();
      Execute<PlayerStickyWallSensorController>();
      Execute<PlayerFinishSensorController>();

      // FSM: the only writer of PlayerModel.State.
      Execute<PlayerStateMachineController>();

      // Actions gated by state.
      Execute<PlayerLaunchController>();
      Execute<PlayerInputGateController>();
      Execute<PlayerPauseController>();

      // Reactive: read state, drive views.
      Execute<PlayerAnimationController>();
      Execute<PlayerFacingController>();
      Execute<PlayerDeathController>();
      Execute<PlayerAudioController>();
      Execute<PlayerLandingDustController>();
      Execute<OffScreenMarkerController>();
      Execute<PlayerEventsBroadcastController>();
    }
  }
}
