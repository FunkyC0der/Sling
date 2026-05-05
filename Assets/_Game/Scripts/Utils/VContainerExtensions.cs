using System;
using Playtika.Controllers;
using Sling.Core;
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
      {
        foreach (Type type in view.GetTypesToRegister())
          builder.RegisterInstance(view, type);
      }
    }
  }
}