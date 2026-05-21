using Sling.Root.Flow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Sling.Editor
{
  [InitializeOnLoad]
  public static class SaveEditorActiveSceneHook
  {
    private const string _kBootScenePath = "Assets/_Game/Scenes/Boot.unity";

    static SaveEditorActiveSceneHook() =>
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.ExitingEditMode)
      {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path == _kBootScenePath)
          return;

        SessionState.SetString(InitFirstSceneController.kEditorActiveSceneSessionKey, activeScene.name);
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_kBootScenePath);
      }
      else if (state == PlayModeStateChange.EnteredEditMode)
      {
        EditorSceneManager.playModeStartScene = null;
        SessionState.EraseString(InitFirstSceneController.kEditorActiveSceneSessionKey);
      }
    }
  }
}
