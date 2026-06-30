using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Boss;
using Sling.Level.Finish;
using Sling.Level.Gameplay;
using Sling.Level.Hud;
using Sling.Level.LevelComplete;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Level
{
  public class LevelScopeController : ScopeControllerWithResultBase<LevelSessionResult>
  {
    public LevelScopeController(IControllerFactory factory, LifetimeScope scope)
      : base(factory, scope)
    {
    }

    protected override string OwnedScopeName => "Level";

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      LevelSessionResult result =
        await ExecuteAndWaitResultAsync<LevelSessionController, LevelSessionResult>(
          GetOwnedControllerFactory(), cancellationToken);

      Complete(result);
    }

    protected override void InitScopeBuilder(IContainerBuilder builder)
    {
      builder.Register<LevelEvents>(Lifetime.Singleton);
      builder.Register<LevelModel>(Lifetime.Singleton);
      
      builder.Register<LevelSessionController>(Lifetime.Transient);
      builder.Register<LevelTrackController>(Lifetime.Transient);
      
      builder.Register<LevelTimeController>(Lifetime.Transient);
      builder.Register<PlayerDeathCountController>(Lifetime.Transient);

      builder.Register<GameplayLoopController>(Lifetime.Transient);

      builder.Register<TryGetUniqueViewFlowController<FinishZoneView>>(Lifetime.Transient);
      builder.Register<LevelCompleteFlowController>(Lifetime.Transient);
      builder.Register<LevelCompleteWindowController>(Lifetime.Transient);

      builder.Register<PlayerScopeController>(Lifetime.Transient);

      builder.Register<SavePlayerStartStatsFlowController>(Lifetime.Transient);

      builder.Register<BossModel>(Lifetime.Singleton);
      builder.Register<BossFlowController>(Lifetime.Transient);
      builder.Register<BossPhaseFlowController>(Lifetime.Transient);
      builder.Register<TryGetUniqueViewFlowController<BossView>>(Lifetime.Transient);

      builder.Register<HudController>(Lifetime.Transient);
      builder.Register<PauseWindowController>(Lifetime.Transient);

      builder.RegisterSceneViews(SceneManager.GetActiveScene());
    }
  }
}
