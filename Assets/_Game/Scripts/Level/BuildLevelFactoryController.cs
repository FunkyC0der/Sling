using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using Sling.Player;
using UnityEngine.SceneManagement;

namespace Sling.Level
{
    public class BuildLevelFactoryController : ControllerWithResultBase<IControllerFactory>
    {
        private readonly PlayerConfig _playerConfig;
    
        public BuildLevelFactoryController(IControllerFactory controllerFactory, PlayerConfig playerConfig)
            : base(controllerFactory)
        {
            _playerConfig = playerConfig;
        }

        protected override UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            Scene scene = SceneManager.GetActiveScene();
            ViewsCollector views = new();
            views.CollectViews(scene);

            if (!views.GetOne<PlayerView>())
            {
                Fail(new System.Exception($"Required view missing in scene '{scene.name}': PlayerView and FinishView are mandatory"));
                return UniTask.CompletedTask;
            }
        
            var events = new LevelEvents();
            IControllerFactory levelFactory = BuildLevelFactory(views, events);
            Complete(levelFactory);
            return UniTask.CompletedTask;
        }

        private IControllerFactory BuildLevelFactory(ViewsCollector views, LevelEvents events)
        {
            var factory = new ControllerFactory();
            factory.Register(() => new LevelLoopController(factory, events));
            factory.Register(() => new PlayerController(factory, views.GetOne<PlayerView>(), _playerConfig));
            return factory;
        }
    }
}