using System;
using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sling.Common.Extensions
{
  public static class VContainerExtensions
  {
    public static IControllerFactory GetControllerFactory(this LifetimeScope scope) =>
      scope.Container.Resolve<IControllerFactory>();

    public static void RegisterSceneViews(this IContainerBuilder builder, Scene scene)
    {
      var viewArraysByType = new Dictionary<Type, List<IViewArrayItem>>();

      // Iterate all MonoBehaviours
      foreach (GameObject root in scene.GetRootGameObjects())
      {
        foreach (MonoBehaviour monoBehaviour in root.GetComponentsInChildren<MonoBehaviour>(includeInactive: true))
        {
          Type componentType = monoBehaviour.GetType();

          if (monoBehaviour is IUniqueView)
            RegisterUnique(builder, componentType, monoBehaviour);
          else if (monoBehaviour is IViewArrayItem viewArrayItem)
            viewArraysByType.GetOrAdd(componentType).Add(viewArrayItem);
        }
      }

      foreach (KeyValuePair<Type, List<IViewArrayItem>> entry in viewArraysByType) 
        RegisterViewList(builder, entry.Key, entry.Value);
    }

    private static void RegisterUnique(IContainerBuilder builder, Type viewType, MonoBehaviour instance)
    {
      if (builder.Exists(viewType))
      {
        throw new InvalidOperationException(
          $"IUniqueView '{viewType.Name}' must have exactly 1 instance in scene, found {instance.name}.");
      }

      builder.RegisterInstance(instance, viewType);
    }

    private static void RegisterViewList(IContainerBuilder builder, Type viewType, List<IViewArrayItem> views)
    {
      var typedArray = Array.CreateInstance(viewType, views.Count);

      for (int i = 0; i < views.Count; i++)
        typedArray.SetValue(views[i], i);

      builder.RegisterInstance(typedArray, viewType.MakeArrayType());
    }

    public static void RegisterGameObjectViews(this IContainerBuilder builder, GameObject gameObject)
    {
      foreach (IGameObjectView view in gameObject.GetComponentsInChildren<IGameObjectView>())
      {
        builder.RegisterInstance(view, view.GetType())
          .AsImplementedInterfaces();
      }
    }
  }
}
