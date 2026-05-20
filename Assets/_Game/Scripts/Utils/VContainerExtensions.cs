using System;
using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Root;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Utils
{
  public static class VContainerExtensions
  {
    public static IControllerFactory GetControllerFactory(this LifetimeScope scope) =>
      scope.Container.Resolve<IControllerFactory>();

    public static void RegisterSceneViews(this IContainerBuilder builder, Scene scene)
    {
      var viewsByType = new Dictionary<Type, List<IView>>();

      foreach (GameObject root in scene.GetRootGameObjects())
      foreach (IView view in root.GetComponentsInChildren<IView>(true))
        viewsByType.GetOrAdd(view.GetType()).Add(view);

      foreach (KeyValuePair<Type, List<IView>> entry in viewsByType)
      {
        // IsAssignableFrom reads backward: checks that entry.Key implements IUniqueView
        bool isUnique = typeof(IUniqueView).IsAssignableFrom(entry.Key);
        if (isUnique)
          RegisterUnique(builder, entry.Key, entry.Value);
        else
          RegisterCollection(builder, entry.Key, entry.Value);
      }
    }

    private static void RegisterUnique(IContainerBuilder builder, Type viewType, List<IView> views)
    {
      if (views.Count != 1)
      {
        throw new InvalidOperationException(
          $"IUniqueView '{viewType.Name}' must have exactly 1 instance in scene, found {views.Count}.");
      }
      
      builder.RegisterInstance(views[0], viewType);
    }

    private static void RegisterCollection(IContainerBuilder builder, Type viewType, List<IView> views)
    {
      var typedArray = Array.CreateInstance(viewType, views.Count);
      
      for (int i = 0; i < views.Count; i++)
        typedArray.SetValue(views[i], i);
      
      builder.RegisterInstance(typedArray, viewType.MakeArrayType());
    }
  }
}
