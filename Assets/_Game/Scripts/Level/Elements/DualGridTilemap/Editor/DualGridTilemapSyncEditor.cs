using Sling.Level.Elements.DualGridTilemap;
using UnityEditor;
using UnityEngine;

namespace Sling.Level.Elements.DualGridTilemap.Editor
{
  [CustomEditor(typeof(DualGridTilemapSync))]
  public class DualGridTilemapSyncEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var sync = (DualGridTilemapSync)target;
      string validationError = sync.GetValidationError();
      string validationWarning = sync.GetValidationWarning();

      EditorGUILayout.Space();

      if (!string.IsNullOrEmpty(validationError))
        EditorGUILayout.HelpBox(validationError, MessageType.Error);
      else if (!string.IsNullOrEmpty(validationWarning))
        EditorGUILayout.HelpBox(validationWarning, MessageType.Warning);

      EditorGUILayout.LabelField("Dual Grid", EditorStyles.boldLabel);

      if (GUILayout.Button("Apply Half-Tile Offset"))
        sync.ApplyHalfTileOffset();

      EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(validationError));
      if (GUILayout.Button("Rebuild Visual"))
        sync.RebuildVisual();
      EditorGUI.EndDisabledGroup();

      EditorGUI.BeginDisabledGroup(sync.VisualTilemap == null);
      if (GUILayout.Button("Clear Visual"))
        sync.ClearVisual();
      EditorGUI.EndDisabledGroup();
    }
  }
}
