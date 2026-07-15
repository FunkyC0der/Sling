using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;

namespace Sling.Editor
{
  [AutoCustomTmxImporter(100)]
  public sealed class TiledMapCenteringImporter : CustomTmxImporter
  {
    public override void TmxAssetImported(TmxAssetImportedArgs args)
    {
      SuperMap map = args.ImportedSuperMap;
      if (map.m_Orientation != MapOrientation.Orthogonal || map.m_Infinite)
        return;

      Grid grid = map.GetComponentInChildren<Grid>(true);
      if (grid == null)
        throw new CustomImporterException($"Map '{map.name}': imported Grid was not found.");

      float inversePixelsPerUnit = args.AssetImporter.InversePPU;
      float width = map.m_Width * map.m_TileWidth * inversePixelsPerUnit;
      float height = map.m_Height * map.m_TileHeight * inversePixelsPerUnit;
      float halfTileHeight = map.m_TileHeight * inversePixelsPerUnit * 0.5f;

      grid.transform.localPosition = new Vector3(-width * 0.5f, height * 0.5f + halfTileHeight, 0f);
    }
  }
}
