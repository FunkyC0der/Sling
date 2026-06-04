using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sling.Level.Elements.DualGridFeature
{
  [CreateAssetMenu(fileName = "DualGridTileSet", menuName = "Game/Dual Grid Tile Set")]
  public class DualGridTileSet : ScriptableObject
  {
    public TileBase Full;
    public TileBase SingleCorner;
    public TileBase EdgeHalf;
    public TileBase DiagonalCorners;
    public TileBase ThreeCorners;
  }
}
