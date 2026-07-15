using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.Controllers;
using Sling.Common.Views;
using Sling.Flow;
using Sling.Infrastructure;
using Sling.Infrastructure.Analytics;
using Sling.Infrastructure.FixedViewport;
using Sling.Infrastructure.Progress;
using Sling.Level;
using Sling.LevelLoading;
using Sling.MainMenu.SelectLevel;
using VContainer;
using VContainer.Unity;

namespace Sling
{
  public class GameScopeController : ScopeControllerBase
  {
    private readonly GameConfig _gameConfig;

    public GameScopeController(IControllerFactory factory, LifetimeScope scope, GameConfig gameConfig)
      : base(factory, scope)
    {
      _gameConfig = gameConfig;
    }

    protected override string OwnedScopeName => "Game";

    protected override void OnStart()
    {
      base.OnStart();
      Execute<GameLoopController>(GetOwnedControllerFactory());
    }

    protected override void InitScopeBuilder(IContainerBuilder builder)
    {
      builder.RegisterInstance(_gameConfig);
      builder.Register<GameModel>(Lifetime.Singleton);

      builder.Register<IViewsProvider, DiViewsProvider>(Lifetime.Scoped);
      builder.Register<IControllerFactory, DiControllerFactory>(Lifetime.Scoped);

      builder.Register<UpdateEvents>(Lifetime.Singleton);
      builder.Register<UpdateController>(Lifetime.Transient);

      builder.RegisterInstance(_gameConfig.FixedViewport);
      builder.Register<FixedViewportController>(Lifetime.Transient);

      builder.RegisterInstance(_gameConfig.AudioConfig);
      builder.Register<AudioEvents>(Lifetime.Singleton);
      builder.Register<AudioController>(Lifetime.Transient);

      builder.Register<GameLoopController>(Lifetime.Transient);
      builder.Register<InitFirstSceneController>(Lifetime.Transient);
      builder.Register<MainMenuStateController>(Lifetime.Transient);
      builder.Register<SelectLevelWindowController>(Lifetime.Transient);
      builder.Register<PlayLevelsStateController>(Lifetime.Transient);
      builder.Register<LoadLevelController>(Lifetime.Transient);
      builder.Register<LevelScopeController>(Lifetime.Transient);

      builder.Register<InitUnityServicesFlowController>(Lifetime.Transient);
      builder.Register<AnalyticsEvents>(Lifetime.Singleton);
      builder.Register<AnalyticsController>(Lifetime.Transient);

      builder.Register<IPlayerProgressStorage, PlayerPrefsPlayerProgressStorage>(Lifetime.Singleton);
      builder.Register<PlayerProgressService>(Lifetime.Singleton);
    }
  }
}
