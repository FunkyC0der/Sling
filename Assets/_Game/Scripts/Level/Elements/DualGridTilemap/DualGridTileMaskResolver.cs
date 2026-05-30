using System;

namespace Sling.Level.Elements.DualGridTilemap
{
  public static class DualGridTileMaskResolver
  {
    public const int BottomLeft = 1;
    public const int BottomRight = 2;
    public const int TopLeft = 4;
    public const int TopRight = 8;

    public static DualGridTileVariant Resolve(int mask)
    {
      if (mask < 0 || mask > 15)
        throw new ArgumentOutOfRangeException(nameof(mask), mask, "Dual-grid mask must be in range 0..15.");

      switch (mask)
      {
        case 0:
          return new DualGridTileVariant(DualGridTileShape.Empty, 0);
        case 15:
          return new DualGridTileVariant(DualGridTileShape.Full, 0);

        case 1:
          return new DualGridTileVariant(DualGridTileShape.SingleCorner, 0);
        case 2:
          return new DualGridTileVariant(DualGridTileShape.SingleCorner, 90);
        case 8:
          return new DualGridTileVariant(DualGridTileShape.SingleCorner, 180);
        case 4:
          return new DualGridTileVariant(DualGridTileShape.SingleCorner, 270);

        case 3:
          return new DualGridTileVariant(DualGridTileShape.EdgeHalf, 0);
        case 10:
          return new DualGridTileVariant(DualGridTileShape.EdgeHalf, 90);
        case 12:
          return new DualGridTileVariant(DualGridTileShape.EdgeHalf, 180);
        case 5:
          return new DualGridTileVariant(DualGridTileShape.EdgeHalf, 270);

        case 9:
          return new DualGridTileVariant(DualGridTileShape.DiagonalSplit, 0);
        case 6:
          return new DualGridTileVariant(DualGridTileShape.DiagonalSplit, 90);

        case 7:
          return new DualGridTileVariant(DualGridTileShape.ThreeCorners, 0);
        case 11:
          return new DualGridTileVariant(DualGridTileShape.ThreeCorners, 90);
        case 14:
          return new DualGridTileVariant(DualGridTileShape.ThreeCorners, 180);
        case 13:
          return new DualGridTileVariant(DualGridTileShape.ThreeCorners, 270);
        default:
          throw new ArgumentOutOfRangeException(nameof(mask), mask, "Unsupported dual-grid mask.");
      }
    }
  }
}
