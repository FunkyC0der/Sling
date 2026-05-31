using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  [CreateAssetMenu(fileName = "DualTilemapGridTileSet", menuName = "Game/Dual Tilemap Grid Tile Set")]
  public class DualTilemapGridTileSet : ScriptableObject
  {
    public TileBase Full;
    public TileBase SingleCorner;
    public TileBase EdgeHalf;
    public TileBase DiagonalCorners;
    public TileBase ThreeCorners;
  }
}
