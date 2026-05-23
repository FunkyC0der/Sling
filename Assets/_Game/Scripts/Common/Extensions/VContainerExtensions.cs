using System;
using System.Collections.Generic;
using System.Reflection;
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
    private static Type[] _cachedNonUniqueViewTypes;

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

      foreach (Type viewType in GetAllNonUniqueViewTypes())
      {
        if (viewsByType.ContainsKey(viewType))
          continue;

        Array emptyArray = Array.CreateInstance(viewType, 0);
        builder.RegisterInstance(emptyArray, viewType.MakeArrayType());
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

    private static Type[] GetAllNonUniqueViewTypes()
    {
      if (_cachedNonUniqueViewTypes != null)
        return _cachedNonUniqueViewTypes;

      var result = new List<Type>();
      Type iViewType = typeof(IView);
      Type iUniqueViewType = typeof(IUniqueView);

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Type[] types;
        try
        {
          types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
          types = ex.Types;
        }

        foreach (Type type in types)
        {
          if (type == null)
            continue;
          if (type.IsInterface || type.IsAbstract)
            continue;
          if (!iViewType.IsAssignableFrom(type))
            continue;
          if (iUniqueViewType.IsAssignableFrom(type))
            continue;

          result.Add(type);
        }
      }

      _cachedNonUniqueViewTypes = result.ToArray();
      return _cachedNonUniqueViewTypes;
    }
  }
}
