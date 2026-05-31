using System;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  public class NeighborFlags
  {
    public const int kNone = 0;
    public const int kBottomLeft = 1;
    public const int kBottomRight = 2;
    public const int kTopRight = 4;
    public const int kTopLeft = 8;
    public const int kFull = kBottomLeft | kBottomRight | kTopRight | kTopLeft;
  }
}
