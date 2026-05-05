using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.StickyWall;
using Sling.Level.WinScreen;
using Sling.Player;
using Sling.Utils;
using UnityEngine;
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
      LifetimeScope levelScope = BuildLevelScope(scene.GetRootGameObjects());

      Complete(levelScope);
      return UniTask.CompletedTask;
    }

    private LifetimeScope BuildLevelScope(GameObject[] sceneRoots)
    {
      return _scope.CreateChild(builder =>
      {
        builder.Register<LevelEvents>(Lifetime.Singleton);

        builder.Register<GameplayLoopController>(Lifetime.Transient);

        builder.Register<FinishController>(Lifetime.Transient);
        builder.Register<WinScreenController>(Lifetime.Transient);

        builder.Register<LaunchController>(Lifetime.Transient);
        builder.Register<StickyWallsController>(Lifetime.Transient);
        
        foreach (GameObject sceneRoot in sceneRoots) 
          builder.RegisterAllViews(sceneRoot);
      });
    }
  }
}
