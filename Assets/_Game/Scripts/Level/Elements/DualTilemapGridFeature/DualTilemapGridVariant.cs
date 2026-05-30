using System;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  public readonly struct DualTilemapGridVariant : IEquatable<DualTilemapGridVariant>
  {
    public DualTilemapGridVariant(DualTilemapGridShape shape, int rotationDegrees)
    {
      Shape = shape;
      RotationDegrees = rotationDegrees;
    }

    public DualTilemapGridShape Shape { get; }
    public int RotationDegrees { get; }
    public bool IsEmpty => Shape == DualTilemapGridShape.Empty;

    public bool Equals(DualTilemapGridVariant other) =>
      Shape == other.Shape && RotationDegrees == other.RotationDegrees;

    public override bool Equals(object obj) =>
      obj is DualTilemapGridVariant other && Equals(other);

    public override int GetHashCode() =>
      ((int)Shape * 397) ^ RotationDegrees;

    public static bool operator ==(DualTilemapGridVariant left, DualTilemapGridVariant right) =>
      left.Equals(right);

    public static bool operator !=(DualTilemapGridVariant left, DualTilemapGridVariant right) =>
      !left.Equals(right);
  }
}
