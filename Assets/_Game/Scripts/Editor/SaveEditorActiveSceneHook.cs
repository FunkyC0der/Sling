using Sling.Flow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Sling.Editor
{
  [InitializeOnLoad]
  public static class SaveEditorActiveSceneHook
  {
    private const string _kBootScenePath = "Assets/_Game/Scenes/Boot.unity";
    private const string _kAutoloadBootScenePrefKey = "Sling.Editor.AutoloadBootScene";
    private const string _kAutoloadBootSceneMenuPath = "Tools/Sling/Autoload Boot Scene";

    static SaveEditorActiveSceneHook() =>
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    [MenuItem(_kAutoloadBootSceneMenuPath)]
    private static void ToggleAutoloadBootScene() =>
      EditorPrefs.SetBool(_kAutoloadBootScenePrefKey, !IsAutoloadBootSceneEnabled());

    [MenuItem(_kAutoloadBootSceneMenuPath, true)]
    private static bool ToggleAutoloadBootSceneValidate()
    {
      Menu.SetChecked(_kAutoloadBootSceneMenuPath, IsAutoloadBootSceneEnabled());
      return true;
    }

    private static bool IsAutoloadBootSceneEnabled() =>
      EditorPrefs.GetBool(_kAutoloadBootScenePrefKey, true);

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.ExitingEditMode)
      {
        if (!IsAutoloadBootSceneEnabled())
          return;

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
