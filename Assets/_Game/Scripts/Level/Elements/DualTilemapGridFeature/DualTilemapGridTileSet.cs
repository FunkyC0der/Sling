using UnityEngine;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  [CreateAssetMenu(fileName = "DualTilemapGridTileSet", menuName = "Game/Dual Tilemap Grid Tile Set")]
  public class DualTilemapGridTileSet : ScriptableObject
  {
    [SerializeField] private Sprite _full;
    [SerializeField] private Sprite _singleCorner;
    [SerializeField] private Sprite _edgeHalf;
    [SerializeField] private Sprite _diagonalSplit;
    [SerializeField] private Sprite _threeCorners;

    public Sprite Full => _full;
    public Sprite SingleCorner => _singleCorner;
    public Sprite EdgeHalf => _edgeHalf;
    public Sprite DiagonalSplit => _diagonalSplit;
    public Sprite ThreeCorners => _threeCorners;
  }
}
