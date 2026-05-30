using System;

namespace Sling.Level.Elements.DualGridTilemap
{
  public readonly struct DualGridTileVariant : IEquatable<DualGridTileVariant>
  {
    public DualGridTileVariant(DualGridTileShape shape, int rotationDegrees)
    {
      Shape = shape;
      RotationDegrees = rotationDegrees;
    }

    public DualGridTileShape Shape { get; }
    public int RotationDegrees { get; }
    public bool IsEmpty => Shape == DualGridTileShape.Empty;

    public bool Equals(DualGridTileVariant other) =>
      Shape == other.Shape && RotationDegrees == other.RotationDegrees;

    public override bool Equals(object obj) =>
      obj is DualGridTileVariant other && Equals(other);

    public override int GetHashCode() =>
      ((int)Shape * 397) ^ RotationDegrees;

    public static bool operator ==(DualGridTileVariant left, DualGridTileVariant right) =>
      left.Equals(right);

    public static bool operator !=(DualGridTileVariant left, DualGridTileVariant right) =>
      !left.Equals(right);
  }
}
