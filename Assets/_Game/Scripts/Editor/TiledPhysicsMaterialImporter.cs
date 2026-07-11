using System.Collections.Generic;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Sling.Editor
{
  [AutoCustomTmxImporter]
  public sealed class TiledPhysicsMaterialImporter : CustomTmxImporter
  {
    private const string _kPhysicsMaterialPropertyName = "physicsMaterial";

    public override void TmxAssetImported(TmxAssetImportedArgs args)
    {
      SuperMap map = args.ImportedSuperMap;
      SuperLayer[] layers = map.GetComponentsInChildren<SuperLayer>(true);

      for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
        ApplyPhysicsMaterial(map, layers[layerIndex]);
    }

    private static void ApplyPhysicsMaterial(SuperMap map, SuperLayer layer)
    {
      if (!layer.gameObject.TryGetCustomPropertySafe(
            _kPhysicsMaterialPropertyName,
            out CustomProperty property))
      {
        return;
      }

      string materialName = property.GetValueAsString();
      if (string.IsNullOrWhiteSpace(materialName))
        throw CreateImportException(map, layer, materialName, "the property value is empty");

      materialName = materialName.Trim();
      PhysicsMaterial2D material = FindPhysicsMaterial(map, layer, materialName);
      Collider2D[] colliders = layer.GetComponentsInChildren<Collider2D>(true);

      for (int colliderIndex = 0; colliderIndex < colliders.Length; colliderIndex++)
      {
        Collider2D collider = colliders[colliderIndex];
        if (collider.GetComponentInParent<SuperLayer>() == layer)
          collider.sharedMaterial = material;
      }
    }

    private static PhysicsMaterial2D FindPhysicsMaterial(
      SuperMap map,
      SuperLayer layer,
      string materialName)
    {
      string[] guids = AssetDatabase.FindAssets($"{materialName} t:PhysicsMaterial2D");
      List<PhysicsMaterial2D> matches = new();

      for (int guidIndex = 0; guidIndex < guids.Length; guidIndex++)
      {
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[guidIndex]);
        PhysicsMaterial2D material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(assetPath);
        if (material != null && material.name == materialName)
          matches.Add(material);
      }

      if (matches.Count == 0)
        throw CreateImportException(map, layer, materialName, "no matching PhysicsMaterial2D asset was found");

      if (matches.Count > 1)
        throw CreateImportException(map, layer, materialName, "multiple matching PhysicsMaterial2D assets were found");

      return matches[0];
    }

    private static CustomImporterException CreateImportException(
      SuperMap map,
      SuperLayer layer,
      string materialName,
      string reason)
    {
      string displayedMaterialName = string.IsNullOrWhiteSpace(materialName)
        ? "<empty>"
        : materialName;

      return new CustomImporterException(
        $"Map '{map.name}', layer '{layer.m_TiledName}': cannot assign physics material " +
        $"'{displayedMaterialName}' because {reason}.");
    }
  }
}
