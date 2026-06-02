using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
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

          builder.Register<GameRootController>(Lifetime.Transient);
          builder.Register<GameScopeController>(Lifetime.Transient);

          builder.Register<IControllerFactory, VContainerControllerFactory>(Lifetime.Scoped);

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
