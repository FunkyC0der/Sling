using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Sling.Level.Elements.DualGridTilemap
{
  [ExecuteAlways]
  [DisallowMultipleComponent]
  public class DualGridTilemapSync : MonoBehaviour
  {
    [SerializeField] private Tilemap _physicalTilemap;
    [SerializeField] private Tilemap _visualTilemap;
    [SerializeField] private DualGridTileSet _tileSet;
    [SerializeField] private bool _autoSync = true;

#if UNITY_EDITOR
    private bool _isSyncing;
    private bool _syncQueued;
#endif

    public Tilemap PhysicalTilemap => _physicalTilemap;
    public Tilemap VisualTilemap => _visualTilemap;
    public DualGridTileSet TileSet => _tileSet;
    public bool AutoSync => _autoSync;

#if UNITY_EDITOR
    private void OnEnable()
    {
      if (!Application.isPlaying)
        Tilemap.tilemapTileChanged += OnTilemapTileChanged;
    }

    private void OnDisable()
    {
      if (!Application.isPlaying)
        Tilemap.tilemapTileChanged -= OnTilemapTileChanged;
    }

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;

      EditorApplication.delayCall -= ApplyHalfTileOffsetIfAlive;
      EditorApplication.delayCall += ApplyHalfTileOffsetIfAlive;
    }

    private void ApplyHalfTileOffsetIfAlive()
    {
      if (this == null)
        return;

      if (_visualTilemap != null)
        ApplyHalfTileOffset();
    }

    private void OnTilemapTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
      if (_isSyncing || !_autoSync || tilemap != _physicalTilemap || syncTiles == null)
        return;

      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
        return;

      QueuePendingSync();
    }

    private void QueuePendingSync()
    {
      if (_syncQueued)
        return;

      _syncQueued = true;
      EditorApplication.delayCall += FlushPendingSync;
    }

    private void FlushPendingSync()
    {
      _syncQueued = false;

      if (this == null)
        return;

      RebuildVisual();
    }
#endif

    public string GetValidationError()
    {
      if (_physicalTilemap == null)
        return "Dual Grid Tilemap Sync requires a physical tilemap.";

      if (_visualTilemap == null)
        return "Dual Grid Tilemap Sync requires a visual tilemap.";

      if (_physicalTilemap == _visualTilemap)
        return "Physical and visual tilemaps must be different.";

      if (_tileSet == null)
        return "Dual Grid Tilemap Sync requires a tile set.";

      string tileSetError = _tileSet.GetValidationError();
      if (!string.IsNullOrEmpty(tileSetError))
        return tileSetError;

      return string.Empty;
    }

    public string GetValidationWarning()
    {
      if (_visualTilemap != null && _visualTilemap.GetComponent<Collider2D>() != null)
        return "Visual tilemap should not have Collider2D components. Keep physics on the physical tilemap.";

      return string.Empty;
    }

    public void ApplyHalfTileOffset()
    {
      if (_visualTilemap == null)
        return;

      GridLayout grid = _visualTilemap.layoutGrid != null
        ? _visualTilemap.layoutGrid
        : _physicalTilemap != null
          ? _physicalTilemap.layoutGrid
          : null;

      Vector3 cellSize = grid != null ? grid.cellSize : Vector3.one;
      Vector3 localPosition = _visualTilemap.transform.localPosition;
      localPosition.x = cellSize.x * 0.5f;
      localPosition.y = cellSize.y * 0.5f;

      if (_visualTilemap.transform.localPosition == localPosition)
        return;

#if UNITY_EDITOR
      Undo.RecordObject(_visualTilemap.transform, "Apply Dual Grid Half-Tile Offset");
#endif
      _visualTilemap.transform.localPosition = localPosition;

#if UNITY_EDITOR
      MarkSceneDirty();
#endif
    }

    public void ClearVisual()
    {
      if (_visualTilemap == null)
        return;

#if UNITY_EDITOR
      Undo.RecordObject(_visualTilemap, "Clear Dual Grid Visual Tilemap");
#endif
      _visualTilemap.ClearAllTiles();

#if UNITY_EDITOR
      MarkSceneDirty();
#endif
    }

    public void RebuildVisual()
    {
      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogError(validationError, this);
        return;
      }

#if UNITY_EDITOR
      _isSyncing = true;
      Undo.RecordObject(_visualTilemap, "Rebuild Dual Grid Visual Tilemap");
#endif
      try
      {
        _visualTilemap.ClearAllTiles();

        BoundsInt bounds = _physicalTilemap.cellBounds;
        for (int y = bounds.yMin - 1; y < bounds.yMax; y++)
        for (int x = bounds.xMin - 1; x < bounds.xMax; x++)
          SyncVisualCell(new Vector3Int(x, y, 0));
      }
      finally
      {
#if UNITY_EDITOR
        _isSyncing = false;
        MarkSceneDirty();
#endif
      }
    }

    public void SyncChangedPhysicalCells(IReadOnlyCollection<Vector3Int> physicalCells)
    {
      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogError(validationError, this);
        return;
      }

#if UNITY_EDITOR
      _isSyncing = true;
      Undo.RecordObject(_visualTilemap, "Sync Dual Grid Visual Tilemap");
#endif
      try
      {
        foreach (Vector3Int physicalCell in physicalCells)
        {
          SyncVisualCell(physicalCell);
          SyncVisualCell(physicalCell - Vector3Int.right);
          SyncVisualCell(physicalCell - Vector3Int.up);
          SyncVisualCell(physicalCell - Vector3Int.right - Vector3Int.up);
        }
      }
      finally
      {
#if UNITY_EDITOR
        _isSyncing = false;
        MarkSceneDirty();
#endif
      }
    }

    public int CalculateMask(Vector3Int visualCell)
    {
      int mask = 0;

      if (HasPhysicalTile(visualCell))
        mask |= DualGridTileMaskResolver.BottomLeft;

      if (HasPhysicalTile(visualCell + Vector3Int.right))
        mask |= DualGridTileMaskResolver.BottomRight;

      if (HasPhysicalTile(visualCell + Vector3Int.up))
        mask |= DualGridTileMaskResolver.TopLeft;

      if (HasPhysicalTile(visualCell + Vector3Int.right + Vector3Int.up))
        mask |= DualGridTileMaskResolver.TopRight;

      return mask;
    }

    private bool HasPhysicalTile(Vector3Int cell) =>
      _physicalTilemap.GetTile(cell) != null;

    private void SyncVisualCell(Vector3Int visualCell)
    {
      int mask = CalculateMask(visualCell);
      if (!_tileSet.TryGetTile(mask, out TileBase tile, out Matrix4x4 transform))
      {
        Debug.LogError("Dual Grid Tile Set is missing a tile for mask " + mask + ".", this);
        return;
      }

      _visualTilemap.SetTile(visualCell, tile);
      _visualTilemap.SetTileFlags(visualCell, TileFlags.None);
      _visualTilemap.SetTransformMatrix(visualCell, tile != null ? transform : Matrix4x4.identity);
    }

#if UNITY_EDITOR
    private void MarkSceneDirty()
    {
      if (_visualTilemap == null)
        return;

      EditorUtility.SetDirty(_visualTilemap);
      EditorSceneManager.MarkSceneDirty(_visualTilemap.gameObject.scene);
    }
#endif
  }
}
