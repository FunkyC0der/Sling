using UnityEditor;
using UnityEngine;

namespace Sling.Level.Elements.RuleTileSpriteGrid.Editor
{
  [CustomEditor(typeof(global::Sling.Level.Elements.RuleTileSpriteGrid.RuleTileObjectGenerator))]
  public class RuleTileObjectGeneratorEditor : UnityEditor.Editor
  {
    private int _width = 1;
    private int _height = 1;

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Generation", EditorStyles.boldLabel);

      _width = Mathf.Max(1, EditorGUILayout.IntField("Width", _width));
      _height = Mathf.Max(1, EditorGUILayout.IntField("Height", _height));

      var generator = (global::Sling.Level.Elements.RuleTileSpriteGrid.RuleTileObjectGenerator)target;
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
