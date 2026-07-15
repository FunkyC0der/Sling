using System;
using UnityEngine;

namespace Sling.Infrastructure.FixedViewport
{
  [Serializable]
  public class FixedViewportConfig
  {
    public Vector2Int ReferenceAspectRatio = new(16, 9);
    public Color BarsColor = Color.black;
  }
}
