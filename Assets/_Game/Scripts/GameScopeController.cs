using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Flow;
using Sling.Infrastructure;
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

    protected override void OnStart()
    {
      base.OnStart();
      Execute<GameLoopController>(GetOwnedControllerFactory());
    }

    protected override void InitScopeBuilder(IContainerBuilder builder)
    {
      builder.RegisterInstance(_gameConfig);
      builder.Register<GameModel>(Lifetime.Singleton);

      builder.Register<GameLoopController>(Lifetime.Transient);
      builder.Register<InitFirstSceneController>(Lifetime.Transient);
      builder.Register<MainMenuStateController>(Lifetime.Transient);
      builder.Register<SelectLevelWindowController>(Lifetime.Transient);
      builder.Register<PlayLevelsStateController>(Lifetime.Transient);
      builder.Register<LoadLevelController>(Lifetime.Transient);
      builder.Register<LevelScopeController>(Lifetime.Transient);

      builder.Register<IControllerFactory, VContainerControllerFactory>(Lifetime.Scoped);
    }
  }
}
