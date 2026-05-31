using PlasticGui.WorkspaceWindow.Security;
using UnityEditor;
using UnityEditor.SceneManagement;
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

      EditorGUILayout.Space();

      if (GUILayout.Button("Align Visual Tilemap"))
      {
        grid.ApplyVisualTilemapOffset();
 
        EditorUtility.SetDirty(grid);
        EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
      }

      if (GUILayout.Button("Rebuild Visual Tilemap"))
      {
        grid.RebuildVisualTilemap();
        
        EditorUtility.SetDirty(grid);
        EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
      }
    }
  }
}
