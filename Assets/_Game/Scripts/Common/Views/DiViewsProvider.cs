using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Sling.Common.Views
{
  public class DiViewsProvider : IViewsProvider
  {
    private readonly IObjectResolver _objectResolver;

    public DiViewsProvider(IObjectResolver objectResolver) => 
      _objectResolver = objectResolver;

    public TView Get<TView>() where TView : Object => 
      _objectResolver.TryResolve(out TView view) ? view : null;

    public List<TView> GetList<TView>() where TView : Object => 
      _objectResolver.TryResolve(out List<TView> views) ? views : null;
  }
}