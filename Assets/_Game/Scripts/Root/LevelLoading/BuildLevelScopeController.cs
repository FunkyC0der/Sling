using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Finish;
using Sling.Level.Gameplay;
using Sling.Level.Hazards;
using Sling.Level.LevelComplete;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Root.LevelLoading
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
      LifetimeScope levelScope = BuildLevelScope(scene);

      Complete(levelScope);
      return UniTask.CompletedTask;
    }

    private LifetimeScope BuildLevelScope(Scene scene)
    {
      return _scope.CreateChild(builder =>
      {
        builder.Register<LevelSessionController>(Lifetime.Transient);

        builder.Register<LevelEvents>(Lifetime.Singleton);
        builder.Register<LevelModel>(Lifetime.Singleton);

        builder.Register<SetPlayerStartPosController>(Lifetime.Transient);
        builder.Register<GameplayLoopController>(Lifetime.Transient);

        builder.Register<LevelCompleteFlowController>(Lifetime.Transient);
        builder.Register<LevelCompleteWindowController>(Lifetime.Transient);
        builder.Register<RespawnPlayerController>(Lifetime.Transient);

        builder.Register<PlayerLaunchController>(Lifetime.Transient);

        builder.Register<HazardZonesController>(Lifetime.Transient);
        builder.Register<FinishZoneController>(Lifetime.Transient);

        builder.RegisterSceneViews(scene);
      },
        childScopeName: "LevelScope");
    }
  }
}
