using UnityEngine;

namespace Sling.Common.Views
{
  public interface IGameObjectView : IView
  {
    // ReSharper disable once InconsistentNaming
    GameObject gameObject { get; }
  }
}