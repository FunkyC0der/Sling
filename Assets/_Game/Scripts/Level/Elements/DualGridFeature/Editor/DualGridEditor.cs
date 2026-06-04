using PlasticGui.WorkspaceWindow.Security;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Sling.Level.Elements.DualGridFeature.Editor
{
  [CustomEditor(typeof(DualGrid))]
  public class DualGridEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var grid = (DualGrid)target;

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
