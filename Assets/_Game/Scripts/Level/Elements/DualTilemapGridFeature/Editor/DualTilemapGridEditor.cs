using UnityEditor;
using UnityEngine;

namespace Sling.Level.Elements.DualTilemapGridFeature.Editor
{
  [CustomEditor(typeof(DualTilemapGrid))]
  public class DualTilemapGridEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var grid = (DualTilemapGrid)target;
      string validationError = grid.GetValidationError();
      string validationWarning = grid.GetValidationWarning();

      EditorGUILayout.Space();

      if (!string.IsNullOrEmpty(validationError))
        EditorGUILayout.HelpBox(validationError, MessageType.Error);
      else if (!string.IsNullOrEmpty(validationWarning))
        EditorGUILayout.HelpBox(validationWarning, MessageType.Warning);

      EditorGUILayout.LabelField("Dual Tilemap Grid", EditorStyles.boldLabel);

      if (GUILayout.Button("Apply Half-Tile Offset"))
        grid.ApplyHalfTileOffset();

      EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(validationError));
      if (GUILayout.Button("Rebuild Visual"))
        grid.RebuildVisual();
      EditorGUI.EndDisabledGroup();

      EditorGUI.BeginDisabledGroup(grid.VisualTilemap == null);
      if (GUILayout.Button("Clear Visual"))
        grid.ClearVisual();
      EditorGUI.EndDisabledGroup();
    }
  }
}
