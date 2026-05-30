using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [SerializeField, HideInInspector] private Tile _fullTile;
    [SerializeField, HideInInspector] private Tile _singleCornerTile;
    [SerializeField, HideInInspector] private Tile _edgeHalfTile;
    [SerializeField, HideInInspector] private Tile _diagonalSplitTile;
    [SerializeField, HideInInspector] private Tile _threeCornersTile;

    public Sprite Full => _full;
    public Sprite SingleCorner => _singleCorner;
    public Sprite EdgeHalf => _edgeHalf;
    public Sprite DiagonalSplit => _diagonalSplit;
    public Sprite ThreeCorners => _threeCorners;

#if UNITY_EDITOR
    private void OnValidate()
    {
      EnsureGeneratedTiles();
    }
#endif

    public bool TryGetTile(int mask, out TileBase tile, out Matrix4x4 transform)
    {
      EnsureGeneratedTiles();

      DualTilemapGridVariant variant = DualTilemapGridMaskResolver.Resolve(mask);
      tile = GetTile(variant.Shape);
      transform = Matrix4x4.TRS(
        Vector3.zero,
        Quaternion.Euler(0f, 0f, variant.RotationDegrees),
        Vector3.one);

      return variant.IsEmpty || tile != null;
    }

    public TileBase GetTile(DualTilemapGridShape shape)
    {
      switch (shape)
      {
        case DualTilemapGridShape.Empty:
          return null;
        case DualTilemapGridShape.Full:
          return _full != null ? _fullTile : null;
        case DualTilemapGridShape.SingleCorner:
          return _singleCorner != null ? _singleCornerTile : null;
        case DualTilemapGridShape.EdgeHalf:
          return _edgeHalf != null ? _edgeHalfTile : null;
        case DualTilemapGridShape.DiagonalSplit:
          return _diagonalSplit != null ? _diagonalSplitTile : null;
        case DualTilemapGridShape.ThreeCorners:
          return _threeCorners != null ? _threeCornersTile : null;
        default:
          return null;
      }
    }

    public string GetValidationError()
    {
      if (_full == null)
        return "Dual Tilemap Grid Tile Set requires a Full sprite.";

      if (_singleCorner == null)
        return "Dual Tilemap Grid Tile Set requires a Single Corner sprite.";

      if (_edgeHalf == null)
        return "Dual Tilemap Grid Tile Set requires an Edge Half sprite.";

      if (_diagonalSplit == null)
        return "Dual Tilemap Grid Tile Set requires a Diagonal Split sprite.";

      if (_threeCorners == null)
        return "Dual Tilemap Grid Tile Set requires a Three Corners sprite.";

      return string.Empty;
    }

    private void EnsureGeneratedTiles()
    {
      UpdateGeneratedTile(ref _fullTile, _full, "DualTilemapGrid_Full");
      UpdateGeneratedTile(ref _singleCornerTile, _singleCorner, "DualTilemapGrid_SingleCorner");
      UpdateGeneratedTile(ref _edgeHalfTile, _edgeHalf, "DualTilemapGrid_EdgeHalf");
      UpdateGeneratedTile(ref _diagonalSplitTile, _diagonalSplit, "DualTilemapGrid_DiagonalSplit");
      UpdateGeneratedTile(ref _threeCornersTile, _threeCorners, "DualTilemapGrid_ThreeCorners");
    }

    private void UpdateGeneratedTile(ref Tile tile, Sprite sprite, string tileName)
    {
      if (sprite == null)
        return;

      if (tile == null)
      {
        tile = CreateInstance<Tile>();
        tile.name = tileName;
        tile.hideFlags = HideFlags.HideInHierarchy;
      }

      tile.sprite = sprite;
      tile.colliderType = Tile.ColliderType.None;

#if UNITY_EDITOR
      AddGeneratedTileToAsset(tile);
      EditorUtility.SetDirty(tile);
#endif
    }

#if UNITY_EDITOR
    private void AddGeneratedTileToAsset(Tile tile)
    {
      if (!EditorUtility.IsPersistent(this) || EditorUtility.IsPersistent(tile))
        return;

      AssetDatabase.AddObjectToAsset(tile, this);
      EditorUtility.SetDirty(this);
    }
#endif
  }
}
