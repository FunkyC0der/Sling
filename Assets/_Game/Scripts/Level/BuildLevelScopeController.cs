using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using Sling.Level.StickyWall;
using Sling.Player;
using Sling.Player.Views;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Level
{
  public class BuildLevelScopeController : ControllerWithResultBase<LifetimeScope>
  {
    private readonly LifetimeScope _scope;

    public BuildLevelScopeController(IControllerFactory factory, LifetimeScope scope)
      : base(factory)
    {
      _scope = scope;
    }

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

      if (!views.GetOne<PlayerInputView>())
      {
        Fail(new System.Exception($"Required view missing in scene '{scene.name}': PlayerInputView is mandatory"));
        return UniTask.CompletedTask;
      }

      Complete(BuildLevelScope(views));
      return UniTask.CompletedTask;
    }

    private LifetimeScope BuildLevelScope(ViewsCollector views)
    {
      return _scope.CreateChild(builder =>
      {
        builder.Register<LevelEvents>(Lifetime.Singleton);

        builder.Register<GameplayLoopController>(Lifetime.Transient);

        builder.RegisterInstance(views.GetOne<PlayerView>());
        builder.RegisterInstance(views.GetOne<PlayerInputView>());
        builder.RegisterInstance(views.GetOne<LaunchTrajectoryView>());

        builder.Register<LaunchController>(Lifetime.Transient);

        builder.Register<StickyWallsController>(Lifetime.Transient)
          .WithParameter<IReadOnlyList<StickyWallView>>(views.GetAll<StickyWallView>());

        builder.RegisterInstance(views.GetOne<FinishView>());
        builder.Register<FinishController>(Lifetime.Transient);
      });
    }
  }
}
