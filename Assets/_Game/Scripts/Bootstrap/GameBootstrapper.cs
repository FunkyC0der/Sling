using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Root.Flow;
using Sling.Root.Game;
using Sling.Root.Infrastructure;
using Sling.Root.LevelLoading;
using Sling.Root.MainMenu.SelectLevel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sling.Bootstrap
{
  public class GameBootstrapper : MonoBehaviour
  {
    [SerializeField] private GameConfig _gameConfig;

    private LifetimeScope _rootScope;

    private void Start()
    {
      DontDestroyOnLoad(gameObject);

      _rootScope = LifetimeScope.Create(builder =>
        {
          builder.RegisterInstance(_gameConfig);
          builder.Register<GameModel>(Lifetime.Singleton);

          builder.Register<GameRootController>(Lifetime.Transient);

          builder.Register<IControllerFactory, VContainerControllerFactory>(Lifetime.Scoped);

          builder.Register<InitFirstSceneController>(Lifetime.Transient);

          builder.Register<MainMenuStateController>(Lifetime.Transient);
          builder.Register<SelectLevelWindowController>(Lifetime.Transient);

          builder.Register<PlayLevelsStateController>(Lifetime.Transient);
          builder.Register<LoadLevelController>(Lifetime.Transient);
          builder.Register<BuildLevelScopeController>(Lifetime.Transient);

          builder.RegisterSceneViews(gameObject.scene);
        },
        name: "Root");

      _rootScope.transform.SetParent(transform);

      var root = _rootScope.Container.Resolve<GameRootController>();
      root.LaunchTree(this.GetCancellationTokenOnDestroy());
    }

    private void OnDestroy() =>
      _rootScope?.Dispose();
  }
}
