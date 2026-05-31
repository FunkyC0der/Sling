using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  public class TileVariant
  {
    public readonly TileBase Tile;
    public readonly Matrix4x4 TransformMatrix;

    public TileVariant(TileBase tile, int rotationDegrees)
    {
      Tile = tile;
      TransformMatrix = Matrix4x4.TRS(
        Vector3.zero,
        Quaternion.Euler(0f, 0f, rotationDegrees),
        Vector3.one);
    }

    public bool IsEmpty => !Tile;
  }
}