using UnityEditor;
using UnityEngine;

namespace Sling.Common.Tweeners.Editor
{
  [CustomEditor(typeof(LinearTrajectorySpriteLine))]
  [CanEditMultipleObjects]
  public class LinearTrajectorySpriteLineEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Space();

      var generator = (LinearTrajectorySpriteLine)target;
      string validationError = generator.GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
        EditorGUILayout.HelpBox(validationError, MessageType.Warning);

      EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(validationError));
      if (GUILayout.Button("Generate Line"))
      {
        serializedObject.ApplyModifiedProperties();

        foreach (Object selectedTarget in targets)
        {
          var line = (LinearTrajectorySpriteLine)selectedTarget;
          line.GenerateLine();
        }
      }
      EditorGUI.EndDisabledGroup();

      if (GUILayout.Button("Clear Line"))
      {
        foreach (Object selectedTarget in targets)
        {
          var line = (LinearTrajectorySpriteLine)selectedTarget;
          line.ClearLine();
        }
      }
    }
  }
}
