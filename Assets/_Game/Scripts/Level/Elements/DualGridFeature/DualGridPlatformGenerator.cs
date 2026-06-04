using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sling.Level.Elements.DualGridFeature
{
  public class DualGridPlatformGenerator : MonoBehaviour
  {
    private const string _kGeneratedVisualTileNamePrefix = "GeneratedVisualTile_";
    private const string _kGeneratedPhysicalTileNamePrefix = "GeneratedPhysicalTile_";
    private const string _kGeneratedRootName = "GeneratedDualGridPlatform";
    private const string _kLegacyGeneratedRootName = "GeneratedDualGridObject";

    [SerializeField] private GameObject _dualGridPrefab;
    [SerializeField] private TileBase _physicalTile;
    [SerializeField] private BoxCollider2D _boxCollider;

#if UNITY_EDITOR
    public void Generate(int width, int height)
    {
      string validationError = GetValidationError(width, height);
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogError(validationError, this);
        return;
      }

      int undoGroup = Undo.GetCurrentGroup();
      Undo.SetCurrentGroupName("Generate Dual Grid Platform");

      GameObject tilemapInstance = null;
      try
      {
        ClearGeneratedTilesInternal(false);

        tilemapInstance = InstantiateTemporaryTilemap();
        DualGrid grid = tilemapInstance.GetComponentInChildren<DualGrid>(true);
        Tilemap physicalTilemap = grid.PhysicalTilemap;
        Tilemap visualTilemap = grid.VisualTilemap;

        FillPhysicalTilemap(physicalTilemap, width, height);
        grid.RebuildVisualTilemap();
        visualTilemap.CompressBounds();

        List<VisualTileData> visualTiles = CollectVisualTiles(tilemapInstance.transform, visualTilemap);
        Vector3 visualCenter = CalculateVisualBoundsCenter(visualTiles);
        Undo.RecordObject(_boxCollider, "Generate Dual Grid Platform");
        ApplyBoxCollider(width, height, GetCellSize(grid));

        GameObject generatedRoot = CreateGeneratedRoot();
        CreateVisualObjects(generatedRoot.transform, visualTiles, visualCenter);
        Undo.RegisterCreatedObjectUndo(generatedRoot, "Generate Dual Grid Platform");

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(_boxCollider);
      }
      finally
      {
        if (tilemapInstance != null)
          DestroyImmediate(tilemapInstance);

        Undo.CollapseUndoOperations(undoGroup);
      }
    }

    public void ClearGeneratedTiles()
    {
      int undoGroup = Undo.GetCurrentGroup();
      Undo.SetCurrentGroupName("Clear Dual Grid Platform");

      ClearGeneratedTilesInternal(true);

      Undo.CollapseUndoOperations(undoGroup);
    }

    private void ClearGeneratedTilesInternal(bool markDirty)
    {
      for (int i = transform.childCount - 1; i >= 0; i--)
      {
        Transform child = transform.GetChild(i);
        if (!IsGeneratedTile(child.name))
          continue;

        Undo.DestroyObjectImmediate(child.gameObject);
      }

      if (markDirty)
        EditorUtility.SetDirty(this);
    }

    public string GetValidationError(int width, int height)
    {
      if (_dualGridPrefab == null)
        return "Dual Grid Platform Generator requires a dual grid prefab.";

      DualGrid grid = _dualGridPrefab.GetComponentInChildren<DualGrid>(true);
      if (grid == null)
        return "Dual grid prefab must contain a DualGrid.";

      if (_physicalTile == null)
        return "Dual Grid Platform Generator requires a physical tile.";

      if (_boxCollider == null)
        return "Dual Grid Platform Generator requires a BoxCollider2D.";

      if (width <= 0)
        return "Width must be greater than zero.";

      if (height <= 0)
        return "Height must be greater than zero.";

      Vector3 cellSize = GetCellSize(grid);
      if (Mathf.Approximately(cellSize.x, 0f) || Mathf.Approximately(cellSize.y, 0f))
        return "Cell size X and Y must be non-zero.";

      return string.Empty;
    }

    private void ApplyBoxCollider(int width, int height, Vector3 cellSize)
    {
      _boxCollider.size = new Vector2(
        Mathf.Abs(width * cellSize.x),
        Mathf.Abs(height * cellSize.y));
      _boxCollider.offset = Vector2.zero;
    }

    private GameObject InstantiateTemporaryTilemap()
    {
      GameObject instance = PrefabUtility.InstantiatePrefab(_dualGridPrefab) as GameObject;
      if (instance == null)
        instance = Instantiate(_dualGridPrefab);

      instance.name = "TemporaryDualGridPlatformGeneratorTilemap";
      SetHideFlags(instance.transform, HideFlags.HideAndDontSave);
      instance.SetActive(true);
      instance.transform.position = transform.position;
      instance.transform.rotation = transform.rotation;
      instance.transform.localScale = transform.localScale;

      return instance;
    }

    private void FillPhysicalTilemap(Tilemap physicalTilemap, int width, int height)
    {
      physicalTilemap.ClearAllTiles();

      for (int y = 0; y < height; y++)
      for (int x = 0; x < width; x++)
        physicalTilemap.SetTile(new Vector3Int(x, y, 0), _physicalTile);

      physicalTilemap.CompressBounds();
    }

    private List<VisualTileData> CollectVisualTiles(Transform temporaryRoot, Tilemap visualTilemap)
    {
      var visualTiles = new List<VisualTileData>();
      BoundsInt bounds = visualTilemap.cellBounds;

      for (int y = bounds.yMin; y < bounds.yMax; y++)
      for (int x = bounds.xMin; x < bounds.xMax; x++)
      {
        Vector3Int cell = new Vector3Int(x, y, 0);
        TileBase tile = visualTilemap.GetTile(cell);
        if (tile == null)
          continue;

        Sprite sprite = visualTilemap.GetSprite(cell);
        if (sprite == null)
          continue;

        visualTiles.Add(new VisualTileData(
          cell,
          sprite,
          GetCellLocalPosition(temporaryRoot, visualTilemap, cell),
          visualTilemap.GetTransformMatrix(cell)));
      }

      return visualTiles;
    }

    private void CreateVisualObjects(Transform generatedRoot, IReadOnlyList<VisualTileData> visualTiles, Vector3 visualCenter)
    {
      for (int i = 0; i < visualTiles.Count; i++)
      {
        VisualTileData visualTile = visualTiles[i];
        GameObject tileObject = CreateVisualTileObject(generatedRoot, visualTile.Cell);
        SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = visualTile.Sprite;

        ApplyVisualTransform(tileObject.transform, visualTile.LocalPosition - visualCenter, visualTile.Transform);
      }
    }

    private GameObject CreateGeneratedRoot()
    {
      var generatedRoot = new GameObject(_kGeneratedRootName);
      generatedRoot.transform.SetParent(transform, false);
      generatedRoot.transform.localPosition = Vector3.zero;
      generatedRoot.transform.localRotation = Quaternion.identity;
      generatedRoot.transform.localScale = Vector3.one;

      return generatedRoot;
    }

    private GameObject CreateVisualTileObject(Transform generatedRoot, Vector3Int cell)
    {
      var tileObject = new GameObject(_kGeneratedVisualTileNamePrefix + cell.x + "_" + cell.y);
      tileObject.transform.SetParent(generatedRoot, false);
      tileObject.transform.localPosition = Vector3.zero;
      tileObject.transform.localRotation = Quaternion.identity;
      tileObject.transform.localScale = Vector3.one;
      tileObject.SetActive(true);

      return tileObject;
    }

    private static Vector3 CalculateVisualBoundsCenter(IReadOnlyList<VisualTileData> visualTiles)
    {
      if (visualTiles.Count == 0)
        return Vector3.zero;

      Vector3 min = visualTiles[0].LocalPosition;
      Vector3 max = visualTiles[0].LocalPosition;
      for (int i = 1; i < visualTiles.Count; i++)
      {
        min = Vector3.Min(min, visualTiles[i].LocalPosition);
        max = Vector3.Max(max, visualTiles[i].LocalPosition);
      }

      return (min + max) * 0.5f;
    }

    private static Vector3 GetCellSize(DualGrid grid)
    {
      GridLayout layoutGrid = null;
      if (grid != null && grid.PhysicalTilemap != null)
        layoutGrid = grid.PhysicalTilemap.layoutGrid;

      if (layoutGrid == null && grid != null && grid.VisualTilemap != null)
        layoutGrid = grid.VisualTilemap.layoutGrid;

      return layoutGrid != null ? layoutGrid.cellSize : Vector3.one;
    }

    private static Vector3 GetCellLocalPosition(Transform temporaryRoot, Tilemap tilemap, Vector3Int cell)
    {
      return GetCellLocalPosition(temporaryRoot, tilemap, cell + tilemap.tileAnchor);
    }

    private static Vector3 GetCellLocalPosition(Transform temporaryRoot, Tilemap tilemap, Vector3 cell)
    {
      Vector3 tilemapLocalPosition = tilemap.CellToLocalInterpolated(cell);
      Vector3 worldPosition = tilemap.transform.TransformPoint(tilemapLocalPosition);
      return temporaryRoot.InverseTransformPoint(worldPosition);
    }

    private static void ApplyVisualTransform(Transform tileTransform, Vector3 localPosition, Matrix4x4 tileMatrix)
    {
      DecomposeMatrix(tileMatrix, out Vector3 matrixPosition, out Quaternion matrixRotation, out Vector3 matrixScale);

      tileTransform.localPosition = localPosition + matrixPosition;
      tileTransform.localRotation = matrixRotation * tileTransform.localRotation;
      tileTransform.localScale = Vector3.Scale(tileTransform.localScale, matrixScale);
    }

    private static void DecomposeMatrix(
      Matrix4x4 matrix,
      out Vector3 position,
      out Quaternion rotation,
      out Vector3 scale)
    {
      position = new Vector3(matrix.m03, matrix.m13, matrix.m23);
      scale = matrix.lossyScale;

      Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22);
      Vector3 upwards = new Vector3(matrix.m01, matrix.m11, matrix.m21);
      rotation = forward.sqrMagnitude > 0f && upwards.sqrMagnitude > 0f
        ? Quaternion.LookRotation(forward, upwards)
        : Quaternion.identity;
    }

    private static bool IsGeneratedTile(string childName) =>
      childName == _kGeneratedRootName ||
      childName == _kLegacyGeneratedRootName ||
      childName.StartsWith(_kGeneratedVisualTileNamePrefix) ||
      childName.StartsWith(_kGeneratedPhysicalTileNamePrefix);

    private static void SetHideFlags(Transform root, HideFlags hideFlags)
    {
      root.gameObject.hideFlags = hideFlags;

      for (int i = 0; i < root.childCount; i++)
        SetHideFlags(root.GetChild(i), hideFlags);
    }

    private readonly struct VisualTileData
    {
      public VisualTileData(Vector3Int cell, Sprite sprite, Vector3 localPosition, Matrix4x4 transform)
      {
        Cell = cell;
        Sprite = sprite;
        LocalPosition = localPosition;
        Transform = transform;
      }

      public Vector3Int Cell { get; }
      public Sprite Sprite { get; }
      public Vector3 LocalPosition { get; }
      public Matrix4x4 Transform { get; }
    }
#endif
  }
}
