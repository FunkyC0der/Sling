using System;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  public static class DualTilemapGridMaskResolver
  {
    public const int BottomLeft = 1;
    public const int BottomRight = 2;
    public const int TopLeft = 4;
    public const int TopRight = 8;

    public static DualTilemapGridVariant Resolve(int mask)
    {
      if (mask < 0 || mask > 15)
        throw new ArgumentOutOfRangeException(nameof(mask), mask, "Dual-tilemap-grid mask must be in range 0..15.");

      switch (mask)
      {
        case 0:
          return new DualTilemapGridVariant(DualTilemapGridShape.Empty, 0);
        case 15:
          return new DualTilemapGridVariant(DualTilemapGridShape.Full, 0);

        case 1:
          return new DualTilemapGridVariant(DualTilemapGridShape.SingleCorner, 0);
        case 2:
          return new DualTilemapGridVariant(DualTilemapGridShape.SingleCorner, 90);
        case 8:
          return new DualTilemapGridVariant(DualTilemapGridShape.SingleCorner, 180);
        case 4:
          return new DualTilemapGridVariant(DualTilemapGridShape.SingleCorner, 270);

        case 3:
          return new DualTilemapGridVariant(DualTilemapGridShape.EdgeHalf, 0);
        case 10:
          return new DualTilemapGridVariant(DualTilemapGridShape.EdgeHalf, 90);
        case 12:
          return new DualTilemapGridVariant(DualTilemapGridShape.EdgeHalf, 180);
        case 5:
          return new DualTilemapGridVariant(DualTilemapGridShape.EdgeHalf, 270);

        case 9:
          return new DualTilemapGridVariant(DualTilemapGridShape.DiagonalSplit, 0);
        case 6:
          return new DualTilemapGridVariant(DualTilemapGridShape.DiagonalSplit, 90);

        case 7:
          return new DualTilemapGridVariant(DualTilemapGridShape.ThreeCorners, 0);
        case 11:
          return new DualTilemapGridVariant(DualTilemapGridShape.ThreeCorners, 90);
        case 14:
          return new DualTilemapGridVariant(DualTilemapGridShape.ThreeCorners, 180);
        case 13:
          return new DualTilemapGridVariant(DualTilemapGridShape.ThreeCorners, 270);
        default:
          throw new ArgumentOutOfRangeException(nameof(mask), mask, "Unsupported dual-grid mask.");
      }
    }
  }
}
