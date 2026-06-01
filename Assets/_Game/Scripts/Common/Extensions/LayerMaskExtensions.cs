using UnityEngine;

namespace Sling.Common.Extensions
{
  public static class LayerMaskExtensions
  {
    public static bool HasLayer(this LayerMask layerMask, int layer) =>
      (layerMask.value & (1 << layer)) != 0;
  }
}
