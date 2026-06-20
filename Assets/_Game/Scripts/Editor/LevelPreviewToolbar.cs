using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Sling.Editor
{
  [Overlay(typeof(SceneView), "Sling Level Previews", true)]
  public class LevelPreviewToolbar : ToolbarOverlay
  {
    public LevelPreviewToolbar() : base(LevelPreviewToolbarButton.kId)
    {
    }
  }

  [EditorToolbarElement(kId, typeof(SceneView))]
  public class LevelPreviewToolbarButton : EditorToolbarButton
  {
    public const string kId = "Sling/RegenerateLevelPreviews";

    public LevelPreviewToolbarButton()
    {
      text = "Level Previews";
      tooltip = "Regenerate previews for every scene listed in GameConfig.Levels.";
      icon = EditorGUIUtility.IconContent("Camera Icon").image as Texture2D;
      clicked += LevelPreviewGenerator.RegenerateAllLevelPreviews;
    }
  }
}
