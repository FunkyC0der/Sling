using UnityEngine;

namespace Sling.Common.Views
{
  public interface IOptionalViewProvider
  {
    TView Get<TView>() where TView : Object;
  }
}
