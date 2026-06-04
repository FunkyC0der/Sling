using UnityEditor;
using UnityEngine;

namespace Sling.Level.Elements.DualGridFeature.Editor
{
  [CustomEditor(typeof(DualGridPlatformGenerator))]
  public class DualGridPlatformGeneratorEditor : UnityEditor.Editor
  {
    private int _width = 1;
    private int _height = 1;

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Generation", EditorStyles.boldLabel);
      
      EditorGUILayout.LabelField("Size in tiles");
      _width = Mathf.Max(1, EditorGUILayout.IntField("Width", _width));
      _height = Mathf.Max(1, EditorGUILayout.IntField("Height", _height));

      var generator = (DualGridPlatformGenerator)target;
      string validationError = generator.GetValidationError(_width, _height);
      if (!string.IsNullOrEmpty(validationError))
        EditorGUILayout.HelpBox(validationError, MessageType.Warning);

      EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(validationError));
      if (GUILayout.Button("Generate"))
        generator.Generate(_width, _height);
      EditorGUI.EndDisabledGroup();

      if (GUILayout.Button("Clear"))
        generator.ClearGeneratedTiles();
    }
  }
}
