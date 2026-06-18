using System;
using System.Collections.Generic;
using Sling.Common.Scenes;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sling.Level.Elements.DualGridFeature.Editor
{
  public class DualGridBatchRebuildWindow : OdinEditorWindow
  {
    [Title("Dual Grid Batch Rebuild")]
    [Required]
    [SerializeField] private DualGridTileSet _tileSet;

    [FolderPath(RequireExistingPath = true)]
    [PropertyOrder(10)]
    [SerializeField] private string _scenesFolder = "Assets/_Game/Scenes/Levels";

    [PropertyOrder(20)]
    [SerializeField] private List<SceneReference> _scenes = new();

    [MenuItem("Tools/Dual Grid/Batch Rebuild Visual Tilemaps")]
    public static void ShowWindow()
    {
      var window = GetWindow<DualGridBatchRebuildWindow>();
      window.titleContent = new GUIContent("Dual Grid Rebuild");
      window.minSize = new Vector2(420f, 320f);
    }

    private string GetValidationError()
    {
      if (EditorApplication.isPlayingOrWillChangePlaymode)
        return "Exit Play Mode before rebuilding Dual Grid scenes.";

      if (_tileSet == null)
        return "Assign a DualGridTileSet.";

      if (GetScenePaths().Count == 0)
        return "Add at least one scene.";

      return string.Empty;
    }

    [Button("Add Scenes From Folder")]
    [PropertyOrder(11)]
    private void AddScenesFromFolder()
    {
      string scenesFolderError = GetScenesFolderError();
      if (!string.IsNullOrEmpty(scenesFolderError))
      {
        Debug.LogWarning("Dual Grid Batch Rebuild: " + scenesFolderError);
        return;
      }

      string folderPath = GetAssetFolderPath(_scenesFolder);
      string[] sceneGuids = AssetDatabase.FindAssets("t:SceneAsset", new[] { folderPath });
      var scenePaths = new List<string>(sceneGuids.Length);

      for (int i = 0; i < sceneGuids.Length; i++)
        scenePaths.Add(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));

      scenePaths.Sort((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));

      for (int i = 0; i < scenePaths.Count; i++)
      {
        string scenePath = scenePaths[i];
        if (ContainsScene(scenePath))
          continue;

        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (sceneAsset != null)
          _scenes.Add(new SceneReference { Scene = sceneAsset });
      }
    }

    [Button(ButtonSizes.Large)]
    [PropertyOrder(30)]
    private void RebuildAll()
    {
      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogWarning("Dual Grid Batch Rebuild: " + validationError);
        return;
      }

      List<string> scenePaths = GetScenePaths();

      if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        return;

      SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();

      try
      {
        for (int i = 0; i < scenePaths.Count; i++)
        {
          string scenePath = scenePaths[i];
          EditorUtility.DisplayProgressBar("Dual Grid Batch Rebuild", scenePath, (float)i / scenePaths.Count);

          Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
          int rebuiltCount = RebuildScene(scene, _tileSet);

          if (rebuiltCount > 0)
          {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
          }
          else
          {
            Debug.LogWarning("Dual Grid Batch Rebuild: no DualGrid tilemaps found in " + scene.path);
          }
        }
      }
      catch (Exception exception)
      {
        Debug.LogError("Dual Grid Batch Rebuild failed: " + exception);
      }
      finally
      {
        EditorUtility.ClearProgressBar();
        EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
      }
    }

    private static int RebuildScene(Scene scene, DualGridTileSet tileSet)
    {
      int rebuiltCount = 0;
      GameObject[] rootObjects = scene.GetRootGameObjects();

      for (int i = 0; i < rootObjects.Length; i++)
      {
        DualGrid[] grids = rootObjects[i].GetComponentsInChildren<DualGrid>(true);
        for (int j = 0; j < grids.Length; j++)
        {
          DualGrid grid = grids[j];
          if (!CanRebuild(grid))
            continue;

          grid.SetTileSetAndRebuildVisualTilemap(tileSet);

          EditorUtility.SetDirty(grid);
          EditorUtility.SetDirty(grid.PhysicalTilemap);
          EditorUtility.SetDirty(grid.VisualTilemap);
          EditorUtility.SetDirty(grid.VisualTilemap.transform);
          PrefabUtility.RecordPrefabInstancePropertyModifications(grid);
          PrefabUtility.RecordPrefabInstancePropertyModifications(grid.PhysicalTilemap);
          PrefabUtility.RecordPrefabInstancePropertyModifications(grid.VisualTilemap);
          PrefabUtility.RecordPrefabInstancePropertyModifications(grid.VisualTilemap.transform);

          rebuiltCount++;
        }
      }

      return rebuiltCount;
    }

    private static bool CanRebuild(DualGrid grid) =>
      grid != null &&
      grid.PhysicalTilemap != null &&
      grid.VisualTilemap != null;

    private List<string> GetScenePaths()
    {
      var scenePaths = new List<string>();

      for (int i = 0; i < _scenes.Count; i++)
      {
        SceneReference sceneReference = _scenes[i];
        if (sceneReference == null || sceneReference.Scene == null)
          continue;

        string scenePath = AssetDatabase.GetAssetPath(sceneReference.Scene);
        if (!string.IsNullOrEmpty(scenePath))
          scenePaths.Add(scenePath);
      }

      return scenePaths;
    }

    private bool ContainsScene(string scenePath)
    {
      for (int i = 0; i < _scenes.Count; i++)
      {
        SceneReference sceneReference = _scenes[i];
        if (sceneReference == null || sceneReference.Scene == null)
          continue;

        if (AssetDatabase.GetAssetPath(sceneReference.Scene) == scenePath)
          return true;
      }

      return false;
    }

    private string GetScenesFolderError()
    {
      string folderPath = GetAssetFolderPath(_scenesFolder);
      if (string.IsNullOrEmpty(folderPath))
        return "Select a folder inside Assets.";

      if (!AssetDatabase.IsValidFolder(folderPath))
        return "Selected scenes folder does not exist.";

      return string.Empty;
    }

    private static string GetAssetFolderPath(string folderPath)
    {
      if (string.IsNullOrEmpty(folderPath))
        return string.Empty;

      folderPath = folderPath.Replace('\\', '/');
      if (folderPath.StartsWith(Application.dataPath, StringComparison.Ordinal))
        return "Assets" + folderPath.Substring(Application.dataPath.Length);

      if (folderPath.StartsWith("Assets", StringComparison.Ordinal))
        return folderPath;

      return string.Empty;
    }
  }
}
