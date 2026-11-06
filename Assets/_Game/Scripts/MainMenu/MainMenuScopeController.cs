using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Controllers;
using Sling.Common.Extensions;
using Sling.Levels;
using Sling.MainMenu.MenuCamera;
using Sling.MainMenu.SelectLevel;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.MainMenu
{
  public class MainMenuScopeController : ScopeControllerWithResultBase<LevelAddress>
  {
    public MainMenuScopeController(IControllerFactory factory, LifetimeScope scope)
      : base(factory, scope)
    {
    }

    protected override string OwnedScopeName => "MainMenu";

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      IControllerFactory factory = GetOwnedControllerFactory();
      Execute<MainMenuCameraController>(factory);

      LevelAddress result =
        await ExecuteAndWaitResultAsync<SelectLevelWindowController, LevelAddress>(factory, cancellationToken);

      Complete(result);
    }

    protected override void InitScopeBuilder(IContainerBuilder builder)
    {
      builder.Register<MainMenuEvents>(Lifetime.Singleton);

      builder.Register<MainMenuCameraController>(Lifetime.Transient);
      builder.Register<SelectLevelWindowController>(Lifetime.Transient);

      // decorative Player / FinishZone copies per biome would break unique scene view registration
      builder.RegisterSceneComponent<MainMenuCameraView>(SceneManager.GetActiveScene());
    }
  }
}
