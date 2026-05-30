using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sling.Level.Elements.DualTilemapGridFeature
{
  [ExecuteAlways]
  [DisallowMultipleComponent]
  public class DualTilemapGrid : MonoBehaviour
  {
    [SerializeField] private Tilemap _physicalTilemap;
    [SerializeField] private Tilemap _visualTilemap;
    [SerializeField] private DualTilemapGridTileSet _tileSet;
    [SerializeField] private bool _autoSync = true;

#if UNITY_EDITOR
    private bool _isSyncing;
    private bool _syncQueued;
#endif

    public Tilemap PhysicalTilemap => _physicalTilemap;
    public Tilemap VisualTilemap => _visualTilemap;
    public DualTilemapGridTileSet TileSet => _tileSet;
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

      EditorApplication.delayCall -= FlushPendingSync;
      _syncQueued = false;
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
        return "Dual Tilemap Grid requires a physical tilemap.";

      if (_visualTilemap == null)
        return "Dual Tilemap Grid requires a visual tilemap.";

      if (_physicalTilemap == _visualTilemap)
        return "Physical and visual tilemaps must be different.";

      if (_tileSet == null)
        return "Dual Tilemap Grid requires a tile set.";

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
      Undo.RecordObject(_visualTilemap.transform, "Apply Dual Tilemap Grid Half-Tile Offset");
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
      Undo.RecordObject(_visualTilemap, "Clear Dual Tilemap Grid Visual Tilemap");
#endif
      _visualTilemap.ClearAllTiles();

#if UNITY_EDITOR
      MarkSceneDirty();
#endif
    }

    public void RebuildVisual()
    {
      RebuildVisual(true);
    }

    public void RebuildVisualWithoutUndo()
    {
      RebuildVisual(false);
    }

    private void RebuildVisual(bool recordUndo)
    {
      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogError(validationError, this);
        return;
      }

#if UNITY_EDITOR
      _isSyncing = true;
      if (recordUndo)
        Undo.RecordObject(_visualTilemap, "Rebuild Dual Tilemap Grid Visual Tilemap");
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
        if (recordUndo)
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
      Undo.RecordObject(_visualTilemap, "Sync Dual Tilemap Grid Visual Tilemap");
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
        mask |= DualTilemapGridMaskResolver.BottomLeft;

      if (HasPhysicalTile(visualCell + Vector3Int.right))
        mask |= DualTilemapGridMaskResolver.BottomRight;

      if (HasPhysicalTile(visualCell + Vector3Int.up))
        mask |= DualTilemapGridMaskResolver.TopLeft;

      if (HasPhysicalTile(visualCell + Vector3Int.right + Vector3Int.up))
        mask |= DualTilemapGridMaskResolver.TopRight;

      return mask;
    }

    private bool HasPhysicalTile(Vector3Int cell) =>
      _physicalTilemap.GetTile(cell) != null;

    private void SyncVisualCell(Vector3Int visualCell)
    {
      int mask = CalculateMask(visualCell);
      if (!_tileSet.TryGetTile(mask, out TileBase tile, out Matrix4x4 transform))
      {
        Debug.LogError("Dual Tilemap Grid Tile Set is missing a tile for mask " + mask + ".", this);
        return;
      }

      if (tile == null)
      {
        _visualTilemap.SetTile(visualCell, null);
        return;
      }

      _visualTilemap.SetTile(visualCell, tile);
      _visualTilemap.SetTileFlags(visualCell, TileFlags.None);
      _visualTilemap.SetTransformMatrix(visualCell, transform);
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
