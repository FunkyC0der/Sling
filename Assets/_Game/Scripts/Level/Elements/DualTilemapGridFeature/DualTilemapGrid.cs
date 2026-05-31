#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif

using UnityEngine;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  [ExecuteAlways]
  public class DualTilemapGrid : MonoBehaviour
  {
#if UNITY_EDITOR
    [SerializeField] private Tilemap _physicalTilemap;
    [SerializeField] private Tilemap _visualTilemap;
    [SerializeField] private DualTilemapGridTileSet _tileSet;
    [SerializeField] private bool _autoSync = true;

    private TileVariant[] _tileVariants;

    public Tilemap PhysicalTilemap => _physicalTilemap;
    public Tilemap VisualTilemap => _visualTilemap;

    private void OnEnable()
    {
      if (Application.isPlaying)
        return;
      
      _tileVariants = CreateTileVariants();
      Tilemap.tilemapTileChanged += OnTilemapTileChanged;
    }

    private void OnDisable()
    {
      if (Application.isPlaying)
        return;

      _tileVariants = null;
      Tilemap.tilemapTileChanged -= OnTilemapTileChanged;
    }

    public void ApplyVisualTilemapOffset() => 
      _visualTilemap.transform.localPosition = GetVisualTilemapOffset();

    public void RebuildVisualTilemap()
    {
      if(!IsVisualTilemapAligned())
        ApplyVisualTilemapOffset();
      
      _visualTilemap.ClearAllTiles();
      
      _physicalTilemap.CompressBounds();
      BoundsInt bounds = _physicalTilemap.cellBounds;

      for (int y = bounds.yMin - 1; y < bounds.yMax; y++)
      for (int x = bounds.xMin - 1; x < bounds.xMax; x++)
      {
        var visualCell = new Vector3Int(x, y, 0);
        
        int mask = CalculateMask(visualCell);

        TileVariant tileVariant = _tileVariants[mask];
        if (tileVariant.IsEmpty)
          continue;

        SetTileToVisualTilemap(visualCell, tileVariant);
      }

      _visualTilemap.RefreshAllTiles();
    }

    private void SetTileToVisualTilemap(Vector3Int cell, TileVariant tileVariant)
    {
      _visualTilemap.SetTile(cell, CreateVisualTile(tileVariant.Sprite));
      _visualTilemap.SetTransformMatrix(cell, tileVariant.TransformMatrix);
    }

    private void OnTilemapTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
      if (!_autoSync || tilemap != _physicalTilemap)
        return;

      EditorApplication.delayCall -= RebuildVisualTilemap;
      EditorApplication.delayCall += RebuildVisualTilemap;
    }

    private Vector3 GetVisualTilemapOffset() => 
      _visualTilemap.layoutGrid.cellSize * 0.5f;

    private bool IsVisualTilemapAligned() => 
      _visualTilemap.transform.localPosition == GetVisualTilemapOffset();

    private static Tile CreateVisualTile(Sprite sprite)
    {
      var tile = ScriptableObject.CreateInstance<Tile>();
      
      tile.hideFlags = HideFlags.HideAndDontSave;
      tile.sprite = sprite;
      tile.colliderType = Tile.ColliderType.None;

      return tile;
    }

    private TileVariant[] CreateTileVariants()
    {
      var variants = new TileVariant[16];

      for (int mask = 0; mask < variants.Length; mask++)
        variants[mask] = CreateTileVariantByMask(mask);

      return variants;
    }

    private int CalculateMask(Vector3Int visualCell)
    {
      var mask = NeighborFlags.kNone;

      if (_physicalTilemap.HasTile(visualCell))
        mask |= NeighborFlags.kBottomLeft;

      if (_physicalTilemap.HasTile(visualCell + Vector3Int.right))
        mask |= NeighborFlags.kBottomRight;

      if (_physicalTilemap.HasTile(visualCell + Vector3Int.up))
        mask |= NeighborFlags.kTopLeft;

      if (_physicalTilemap.HasTile(visualCell + Vector3Int.right + Vector3Int.up))
        mask |= NeighborFlags.kTopRight;

      return (int)mask;
    }

    private TileVariant CreateTileVariantByMask(int mask)
    {
      if (mask < NeighborFlags.kNone || mask > 15)
        throw new ArgumentOutOfRangeException(nameof(mask), mask, "Dual-tilemap-grid mask must be in range 0..15.");

      switch (mask)
      {
        case NeighborFlags.kNone:
          return new TileVariant(null, 0);

        case NeighborFlags.kBottomLeft:
          return new TileVariant(_tileSet.SingleCorner, 0);
        case NeighborFlags.kBottomRight:
          return new TileVariant(_tileSet.SingleCorner, 90);
        case NeighborFlags.kTopRight:
          return new TileVariant(_tileSet.SingleCorner, 180);
        case NeighborFlags.kTopLeft:
          return new TileVariant(_tileSet.SingleCorner, 270);

        case NeighborFlags.kBottomLeft | NeighborFlags.kBottomRight:
          return new TileVariant(_tileSet.EdgeHalf, 0);
        case NeighborFlags.kBottomRight | NeighborFlags.kTopRight:
          return new TileVariant(_tileSet.EdgeHalf, 90);
        case NeighborFlags.kTopRight | NeighborFlags.kTopLeft:
          return new TileVariant(_tileSet.EdgeHalf, 180);
        case NeighborFlags.kTopLeft | NeighborFlags.kBottomLeft:
          return new TileVariant(_tileSet.EdgeHalf, 270);

        case NeighborFlags.kBottomLeft | NeighborFlags.kTopRight:
          return new TileVariant(_tileSet.DiagonalSplit, 0);
        case NeighborFlags.kBottomRight | NeighborFlags.kTopLeft:
          return new TileVariant(_tileSet.DiagonalSplit, 90);

        case NeighborFlags.kFull & ~NeighborFlags.kTopRight:
          return new TileVariant(_tileSet.ThreeCorners, 0);
        case NeighborFlags.kFull & ~NeighborFlags.kTopLeft:
          return new TileVariant(_tileSet.ThreeCorners, 90);
        case NeighborFlags.kFull & ~NeighborFlags.kBottomLeft:
          return new TileVariant(_tileSet.ThreeCorners, 180);
        case NeighborFlags.kFull & ~NeighborFlags.kBottomRight:
          return new TileVariant(_tileSet.ThreeCorners, 270);
        
        case NeighborFlags.kFull:
          return new TileVariant(_tileSet.Full, 0);
        
        default:
          throw new ArgumentOutOfRangeException(nameof(mask), mask, "Unsupported dual-grid mask.");
      }
    }
#endif
  }
}
