using System.Collections.Generic;
using UnityEngine;

namespace Sling.Common.Views
{
  public interface IViewsProvider
  {
    TView Get<TView>() where TView : Object;
    List<TView> GetList<TView>() where TView : Object;
  }
}