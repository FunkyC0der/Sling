using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sling.Boot
{
  public class GameBootstrapper : MonoBehaviour
  {
    private LifetimeScope _rootScope;

    private void Start()
    {
      DontDestroyOnLoad(gameObject);
      
      _rootScope = LifetimeScope.Create(builder =>
      {
        builder.Register<GameRootController>(Lifetime.Transient);

        builder.Register<GameModel>(Lifetime.Singleton);
        
        builder.Register<IControllerFactory, VContainerControllerFactory>(Lifetime.Scoped);

        builder.Register<InitFirstSceneController>(Lifetime.Transient);
        
        builder.Register<PlayLevelsStateController>(Lifetime.Transient);
        builder.Register<LoadLevelController>(Lifetime.Transient);
        builder.Register<BuildLevelScopeController>(Lifetime.Transient);
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