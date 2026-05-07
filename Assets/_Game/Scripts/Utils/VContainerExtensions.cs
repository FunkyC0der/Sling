using Playtika.Controllers;
using Sling.Core;
using Sling.Level.Hazards;
using Sling.Level.StickyWall;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sling.Utils
{
  public static class VContainerExtensions
  {
    public static IControllerFactory GetControllerFactory(this LifetimeScope scope) =>
      scope.Container.Resolve<IControllerFactory>();

    public static void RegisterAllViews(this IContainerBuilder builder, GameObject root)
    {
      foreach (BaseView view in root.GetComponentsInChildren<BaseView>()) 
        builder.RegisterInstance(view, view.GetType());

      // TODO: Thing how to make it better.
      HazardZoneView[] hazardZones = root.GetComponentsInChildren<HazardZoneView>();
      if(hazardZones.Length > 0)
        builder.RegisterInstance(hazardZones);

      StickyWallView[] stickyWalls = root.GetComponentsInChildren<StickyWallView>();
      if(stickyWalls.Length > 0)
        builder.RegisterInstance(stickyWalls);
    }
  }
}