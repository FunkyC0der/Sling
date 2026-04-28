using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using Sling.Level.StickyWall;
using Sling.Player;
using Sling.Player.Trajectory;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Level
{
    public class BuildLevelFactoryController : ControllerWithResultBase<LifetimeScope>
    {
        private readonly LifetimeScope _scope;

        public BuildLevelFactoryController(IControllerFactory factory, LifetimeScope scope) 
            : base(factory) =>
            _scope = scope;

        protected override UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            Scene scene = SceneManager.GetActiveScene();
            ViewsCollector views = new();
            views.CollectViews(scene);

            if (!views.GetOne<PlayerView>())
            {
                Fail(new System.Exception($"Required view missing in scene '{scene.name}': PlayerView is mandatory"));
                return UniTask.CompletedTask;
            }

            Complete(BuildLevelScope(views));
            return UniTask.CompletedTask;
        }

        private LifetimeScope BuildLevelScope(ViewsCollector views) =>
            _scope.CreateChild(builder =>
            {
                builder.Register<LevelEvents>(Lifetime.Singleton);
                
                builder.Register<LevelLoopController>(Lifetime.Transient);
                
                builder.RegisterInstance(views.GetOne<PlayerView>());
                builder.Register<PlayerController>(Lifetime.Transient);
                builder.Register<TrajectoryController>(Lifetime.Transient);
                
                builder.RegisterInstance(views.GetOne<TrajectoryView>());

                foreach (StickyWallView stickyWall in views.GetAll<StickyWallView>()) 
                    builder.RegisterInstance(stickyWall);
                
                builder.Register<StickyWallsController>(Lifetime.Transient);
            });
    }
}
