using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Collision;
using Sling.Level.Gameplay;
using Sling.Level.Player.LandingDust;
using Sling.Level.Player.Launch;
using Sling.Level.Player.States;
using VContainer;
using VContainer.Unity;

namespace Sling.Level.Player
{
  public class PlayerScopeController : ScopeControllerBase
  {
    private readonly PlayerView _playerView;

    public PlayerScopeController(IControllerFactory controllerFactory, LifetimeScope scope, PlayerView playerView) 
      : base(controllerFactory, scope)
    {
      _playerView = playerView;
    }

    protected override string OwnedScopeName => "Player";

    protected override void OnStart()
    {
      base.OnStart();
      Execute<PlayerController>(GetOwnedControllerFactory());
    }

    protected override void InitScopeBuilder(IContainerBuilder builder)
    {
      builder.RegisterInstance(_playerView.Config);
      
      builder.Register<PlayerModel>(Lifetime.Singleton);
      builder.Register<PlayerController>(Lifetime.Transient);
      
      builder.Register<PlayerAnimatorController>(Lifetime.Transient);

      builder.Register<PlayerStatesController>(Lifetime.Transient);
      builder.Register<IsInAirController>(Lifetime.Transient);
      builder.Register<IsWallSlidingController>(Lifetime.Transient);

      builder.Register<PlayerLaunchController>(Lifetime.Transient);
      builder.Register<PlayerPreLaunchFlowController>(Lifetime.Transient);

      builder.Register<PlayerDeathController>(Lifetime.Transient);

      builder.Register<PlayerAudioController>(Lifetime.Transient);
      builder.Register<PlayerLandingDustController>(Lifetime.Transient);
      
      builder.RegisterGameObjectViews(_playerView.gameObject);
    }
  }
}
