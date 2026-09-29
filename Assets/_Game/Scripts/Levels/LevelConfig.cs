using System;
using Sirenix.OdinInspector;
using Sling.Audio;
using Sling.Common.Scenes;
using UnityEngine;

namespace Sling.Levels
{
  [InlineProperty]
  [Serializable]
  public class LevelConfig
  {
    [OnInspectorGUI("DrawIndex")]
    public Texture2D Preview;
    public LevelType Type;
    [OnValueChanged("RefreshPreview")]
    public SceneReference Scene;
    public AudioClipId Track = AudioClipId.LevelTrack1;

#if UNITY_EDITOR
    private static void DrawIndex(Sirenix.OdinInspector.Editor.InspectorProperty property)
    {
      var index = property.ParentValueProperty?.Index + 1 ?? -1;
      GUILayout.Label(index.ToString(), GUILayout.Width(18));
    }

    private void RefreshPreview()
    {
      if (Scene?.Scene == null)
      {
        Preview = null;
        return;
      }

      var previewPath = "Assets/_Game/Generated/LevelPreviews/" + Scene.Scene.name + ".png";
      Preview = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(previewPath);
    }
#endif
  }
}
