using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Common
{
  public interface IPositionView : IGameObjectView
  {
    Vector3 Position { get; }
  }
}