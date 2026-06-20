using System.Collections.Generic;
using System.IO;
using Sling.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sling.Editor
{
  [InitializeOnLoad]
  public static class LevelPreviewGenerator
  {
    private const int _kPreviewWidth = 320;
    private const int _kPreviewHeight = 180;
    private const string _kPreviewFolderPath = "Assets/_Game/Generated/LevelPreviews";

    static LevelPreviewGenerator() =>
      EditorSceneManager.sceneSaved += OnSceneSaved;

    [MenuItem("Tools/Sling/Regenerate Open Level Preview")]
    private static void RegenerateOpenLevelPreview() =>
      GeneratePreviewForScene(SceneManager.GetActiveScene());

    [MenuItem("Tools/Sling/Regenerate All Level Previews")]
    public static void RegenerateAllLevelPreviews()
    {
      if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        return;

      SceneSetup[] previousSceneSetup = EditorSceneManager.GetSceneManagerSetup();

      try
      {
        List<GameConfig> gameConfigs = FindGameConfigs();
        List<string> scenePaths = GetLevelScenePaths(gameConfigs);
        for (int sceneIndex = 0; sceneIndex < scenePaths.Count; sceneIndex++)
        {
          string scenePath = scenePaths[sceneIndex];
          EditorUtility.DisplayProgressBar(
            "Generating Level Previews",
            scenePath,
            (float)sceneIndex / scenePaths.Count);

          Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
          GeneratePreviewForScene(scene, gameConfigs);
        }
      }
      finally
      {
        EditorUtility.ClearProgressBar();

        if (previousSceneSetup.Length > 0)
          EditorSceneManager.RestoreSceneManagerSetup(previousSceneSetup);
      }
    }

    private static void OnSceneSaved(Scene scene) =>
      GeneratePreviewForScene(scene);

    private static void GeneratePreviewForScene(Scene scene) =>
      GeneratePreviewForScene(scene, FindGameConfigs());

    private static void GeneratePreviewForScene(Scene scene, List<GameConfig> gameConfigs)
    {
      if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
        return;

      Camera previewCamera = FindPreviewCamera(scene);
      if (previewCamera == null)
        return;

      bool changed = false;
      Texture2D preview = RenderPreview(previewCamera, scene.name);
      if (preview == null)
        return;

      for (int configIndex = 0; configIndex < gameConfigs.Count; configIndex++)
      {
        GameConfig gameConfig = gameConfigs[configIndex];
        if (gameConfig == null)
          continue;

        for (int levelIndex = 0; levelIndex < gameConfig.Levels.Count; levelIndex++)
        {
          LevelConfig levelConfig = gameConfig.Levels[levelIndex];
          if (levelConfig.Scene == null || levelConfig.Scene.Scene == null)
            continue;

          string levelScenePath = AssetDatabase.GetAssetPath(levelConfig.Scene.Scene);
          if (levelScenePath != scene.path)
            continue;

          levelConfig.Preview = preview;
          EditorUtility.SetDirty(gameConfig);
          changed = true;
        }
      }

      if (changed)
        AssetDatabase.SaveAssets();
    }

    private static List<GameConfig> FindGameConfigs()
    {
      List<GameConfig> gameConfigs = new();
      string[] configGuids = AssetDatabase.FindAssets("t:GameConfig");
      for (int configIndex = 0; configIndex < configGuids.Length; configIndex++)
      {
        string configPath = AssetDatabase.GUIDToAssetPath(configGuids[configIndex]);
        GameConfig gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(configPath);
        if (gameConfig != null)
          gameConfigs.Add(gameConfig);
      }

      return gameConfigs;
    }

    private static List<string> GetLevelScenePaths(List<GameConfig> gameConfigs)
    {
      List<string> scenePaths = new();
      HashSet<string> addedScenePaths = new();

      for (int configIndex = 0; configIndex < gameConfigs.Count; configIndex++)
      {
        GameConfig gameConfig = gameConfigs[configIndex];
        for (int levelIndex = 0; levelIndex < gameConfig.Levels.Count; levelIndex++)
        {
          LevelConfig levelConfig = gameConfig.Levels[levelIndex];
          if (levelConfig.Scene == null || levelConfig.Scene.Scene == null)
            continue;

          string scenePath = AssetDatabase.GetAssetPath(levelConfig.Scene.Scene);
          if (!string.IsNullOrEmpty(scenePath) && addedScenePaths.Add(scenePath))
            scenePaths.Add(scenePath);
        }
      }

      return scenePaths;
    }

    private static Camera FindPreviewCamera(Scene scene)
    {
      GameObject[] rootGameObjects = scene.GetRootGameObjects();
      Camera fallbackCamera = null;

      for (int i = 0; i < rootGameObjects.Length; i++)
      {
        Camera[] cameras = rootGameObjects[i].GetComponentsInChildren<Camera>(true);
        for (int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
        {
          Camera camera = cameras[cameraIndex];
          if (fallbackCamera == null)
            fallbackCamera = camera;

          if (!camera.isActiveAndEnabled)
            continue;

          if (camera.CompareTag("MainCamera"))
            return camera;

          fallbackCamera = camera;
        }
      }

      return fallbackCamera;
    }

    private static Texture2D RenderPreview(Camera camera, string sceneName)
    {
      Directory.CreateDirectory(_kPreviewFolderPath);

      string previewPath = _kPreviewFolderPath + "/" + sceneName + ".png";
      RenderTexture renderTexture =
        RenderTexture.GetTemporary(_kPreviewWidth, _kPreviewHeight, 24, RenderTextureFormat.ARGB32);
      Texture2D texture = new Texture2D(_kPreviewWidth, _kPreviewHeight, TextureFormat.RGBA32, false);

      RenderTexture previousActive = RenderTexture.active;
      RenderTexture previousTargetTexture = camera.targetTexture;
      float previousAspect = camera.aspect;

      try
      {
        camera.targetTexture = renderTexture;
        camera.aspect = (float)_kPreviewWidth / _kPreviewHeight;
        camera.Render();

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, _kPreviewWidth, _kPreviewHeight), 0, 0);
        texture.Apply();

        File.WriteAllBytes(previewPath, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(previewPath);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(previewPath);
      }
      finally
      {
        camera.targetTexture = previousTargetTexture;
        camera.aspect = previousAspect;
        RenderTexture.active = previousActive;
        RenderTexture.ReleaseTemporary(renderTexture);
        Object.DestroyImmediate(texture);
      }
    }
  }
}
