using UnityEngine;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  public class TileVariant
  {
    public readonly Sprite Sprite;
    public readonly Matrix4x4 TransformMatrix;

    public TileVariant(Sprite sprite, int rotationDegrees)
    {
      Sprite = sprite;
      TransformMatrix = Matrix4x4.TRS(
        Vector3.zero,
        Quaternion.Euler(0f, 0f, rotationDegrees),
        Vector3.one);
    }

    public bool IsEmpty => !Sprite;
  }
}