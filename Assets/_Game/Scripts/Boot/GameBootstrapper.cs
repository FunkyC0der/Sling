using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using Sling.Level;
using Sling.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sling.Boot
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _playerConfig;

        private LifetimeScope _rootScope;

        private void Start()
        {
            _rootScope = LifetimeScope.Create(builder =>
            {
                builder.RegisterInstance(_playerConfig);

                builder.Register<BootstrapController>(Lifetime.Transient);
                builder.Register<GameLoopController>(Lifetime.Transient);
                builder.Register<LevelSessionController>(Lifetime.Transient);
                builder.Register<BuildLevelFactoryController>(Lifetime.Transient);
                builder.Register<GameRootController>(Lifetime.Transient);

                builder.Register<IControllerFactory, VContainerControllerFactory>(Lifetime.Scoped);
            });

            var root = _rootScope.Container.Resolve<GameRootController>();
            root.LaunchTree(this.GetCancellationTokenOnDestroy());
        }

        private void OnDestroy()
        {
            _rootScope?.Dispose();
        }
    }
}
