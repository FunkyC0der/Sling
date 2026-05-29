using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Level.Elements.RuleTileSpriteGrid
{
  public class RuleTileObjectGenerator : MonoBehaviour
  {
    private const string GeneratedTileNamePrefix = "GeneratedTile_";

    [SerializeField] private RuleTile _ruleTile;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _tilemapPrefab;

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
      Undo.SetCurrentGroupName("Generate RuleTile Sprite Grid");

      GameObject tilemapInstance = null;
      try
      {
        ClearGeneratedTiles();

        tilemapInstance = InstantiateTemporaryTilemap();
        Tilemap tilemap = tilemapInstance.GetComponentInChildren<Tilemap>(true);
        FillTemporaryTilemap(tilemap, width, height);
        CreateTileObjects(tilemapInstance.transform, tilemap, width, height);

        EditorUtility.SetDirty(this);
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
      for (int i = transform.childCount - 1; i >= 0; i--)
      {
        Transform child = transform.GetChild(i);
        if (!child.name.StartsWith(GeneratedTileNamePrefix))
          continue;

        Undo.DestroyObjectImmediate(child.gameObject);
      }

      EditorUtility.SetDirty(this);
    }

    public string GetValidationError(int width, int height)
    {
      if (_ruleTile == null)
        return "RuleTile Sprite Grid requires a RuleTile.";

      if (_tilePrefab == null)
        return "RuleTile Sprite Grid requires a tile prefab.";

      if (_tilePrefab.GetComponentInChildren<SpriteRenderer>(true) == null)
        return "Tile prefab must contain a SpriteRenderer.";

      if (_tilemapPrefab == null)
        return "RuleTile Sprite Grid requires a tilemap prefab.";

      if (_tilemapPrefab.GetComponentInChildren<Grid>(true) == null)
        return "Tilemap prefab must contain a Grid.";

      if (_tilemapPrefab.GetComponentInChildren<Tilemap>(true) == null)
        return "Tilemap prefab must contain a Tilemap.";

      if (width <= 0)
        return "Width must be greater than zero.";

      if (height <= 0)
        return "Height must be greater than zero.";

      return string.Empty;
    }

    private GameObject InstantiateTemporaryTilemap()
    {
      GameObject instance = PrefabUtility.InstantiatePrefab(_tilemapPrefab) as GameObject;
      if (instance == null)
        instance = Instantiate(_tilemapPrefab);

      instance.name = "TemporaryRuleTileObjectGeneratorTilemap";
      SetHideFlags(instance.transform, HideFlags.HideAndDontSave);
      instance.SetActive(true);
      instance.transform.position = transform.position;
      instance.transform.rotation = transform.rotation;
      instance.transform.localScale = transform.localScale;

      return instance;
    }

    private void FillTemporaryTilemap(Tilemap tilemap, int width, int height)
    {
      tilemap.ClearAllTiles();

      for (int y = 0; y < height; y++)
      for (int x = 0; x < width; x++)
        tilemap.SetTile(new Vector3Int(x, y, 0), _ruleTile);

      tilemap.RefreshAllTiles();
    }

    private void CreateTileObjects(Transform temporaryRoot, Tilemap tilemap, int width, int height)
    {
      Vector3 gridCenter = CalculateGridCenter(temporaryRoot, tilemap, width, height);

      for (int y = 0; y < height; y++)
      for (int x = 0; x < width; x++)
      {
        Vector3Int cell = new Vector3Int(x, y, 0);
        TileData tileData = new TileData();
        _ruleTile.GetTileData(cell, tilemap, ref tileData);

        GameObject tileObject = CreateTileObject(cell);
        SpriteRenderer spriteRenderer = tileObject.GetComponentInChildren<SpriteRenderer>(true);
        spriteRenderer.sprite = tileData.sprite;

        ApplyTileTransform(tileObject.transform, temporaryRoot, tilemap, cell, gridCenter, tileData.transform);
      }
    }

    private GameObject CreateTileObject(Vector3Int cell)
    {
      GameObject tileObject = PrefabUtility.InstantiatePrefab(_tilePrefab, transform) as GameObject;
      if (tileObject == null)
      {
        tileObject = Instantiate(_tilePrefab, transform);
        Undo.RegisterCreatedObjectUndo(tileObject, "Create RuleTile Sprite");
      }
      else
      {
        Undo.RegisterCreatedObjectUndo(tileObject, "Create RuleTile Sprite");
      }

      tileObject.name = GeneratedTileNamePrefix + cell.x + "_" + cell.y;
      tileObject.SetActive(true);

      return tileObject;
    }

    private static void ApplyTileTransform(
      Transform tileTransform,
      Transform temporaryRoot,
      Tilemap tilemap,
      Vector3Int cell,
      Vector3 gridCenter,
      Matrix4x4 tileMatrix)
    {
      Vector3 cellLocalPosition = GetCellLocalPosition(temporaryRoot, tilemap, cell);
      DecomposeMatrix(tileMatrix, out Vector3 matrixPosition, out Quaternion matrixRotation, out Vector3 matrixScale);

      tileTransform.localPosition = cellLocalPosition - gridCenter + matrixPosition;
      tileTransform.localRotation = matrixRotation * tileTransform.localRotation;
      tileTransform.localScale = Vector3.Scale(tileTransform.localScale, matrixScale);
    }

    private static Vector3 CalculateGridCenter(Transform temporaryRoot, Tilemap tilemap, int width, int height)
    {
      Vector3 min = GetCellLocalPosition(temporaryRoot, tilemap, Vector3Int.zero);
      Vector3 max = min;

      for (int y = 0; y < height; y++)
      for (int x = 0; x < width; x++)
      {
        Vector3 position = GetCellLocalPosition(temporaryRoot, tilemap, new Vector3Int(x, y, 0));
        min = Vector3.Min(min, position);
        max = Vector3.Max(max, position);
      }

      return (min + max) * 0.5f;
    }

    private static Vector3 GetCellLocalPosition(Transform temporaryRoot, Tilemap tilemap, Vector3Int cell)
    {
      Vector3 tilemapLocalPosition = tilemap.CellToLocalInterpolated(cell + tilemap.tileAnchor);
      Vector3 worldPosition = tilemap.transform.TransformPoint(tilemapLocalPosition);
      return temporaryRoot.InverseTransformPoint(worldPosition);
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

    private static void SetHideFlags(Transform root, HideFlags hideFlags)
    {
      root.gameObject.hideFlags = hideFlags;

      for (int i = 0; i < root.childCount; i++)
        SetHideFlags(root.GetChild(i), hideFlags);
    }
#endif
  }
}
