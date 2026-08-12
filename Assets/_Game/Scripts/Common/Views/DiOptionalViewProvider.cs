using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Sling.Common.Views
{
  public class DiOptionalViewProvider : IOptionalViewProvider
  {
    private readonly IObjectResolver _objectResolver;

    public DiOptionalViewProvider(IObjectResolver objectResolver) =>
      _objectResolver = objectResolver;

    public TView Get<TView>() where TView : UnityEngine.Object =>
      _objectResolver.TryResolve(out TView view) ? view : null;

    public IReadOnlyList<TView> GetAll<TView>() where TView : class =>
      _objectResolver.TryResolve(out List<TView> views) ? views : Array.Empty<TView>();
  }
}
