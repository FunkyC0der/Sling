using UnityEngine;
using VContainer;

namespace Sling.Common.Views
{
  public class DiOptionalViewProvider : IOptionalViewProvider
  {
    private readonly IObjectResolver _objectResolver;

    public DiOptionalViewProvider(IObjectResolver objectResolver) =>
      _objectResolver = objectResolver;

    public TView Get<TView>() where TView : Object =>
      _objectResolver.TryResolve(out TView view) ? view : null;
  }
}
