using System;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  [Flags]
  public enum TileVariantsMask
  {
    None = 0,
    BottomLeft = 1,
    BottomRight = 2,
    TopLeft = 4,
    TopRight = 8
  }
}
