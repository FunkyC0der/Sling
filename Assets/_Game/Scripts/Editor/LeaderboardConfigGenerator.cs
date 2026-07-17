using System.Collections.Generic;
using System.IO;
using Sling.Levels;
using UnityEditor;
using UnityEngine;

namespace Sling.Editor
{
  public static class LeaderboardConfigGenerator
  {
    private const string _kLeaderboardConfigsFolderPath = "Assets/_Game/Configs/Leaderboards";

    [MenuItem("Tools/Sling/Create Missing Leaderboard Configs")]
    public static void CreateMissingLeaderboardConfigs()
    {
      bool folderCreated = !Directory.Exists(_kLeaderboardConfigsFolderPath);
      Directory.CreateDirectory(_kLeaderboardConfigsFolderPath);

      int createdCount = 0;
      HashSet<string> processedSceneNames = new();
      string[] gameConfigGuids = AssetDatabase.FindAssets("t:GameConfig");

      for (int configIndex = 0; configIndex < gameConfigGuids.Length; configIndex++)
      {
        string configPath = AssetDatabase.GUIDToAssetPath(gameConfigGuids[configIndex]);
        GameConfig gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(configPath);
        if (gameConfig == null || gameConfig.Levels == null)
          continue;

        for (int levelIndex = 0; levelIndex < gameConfig.Levels.Count; levelIndex++)
        {
          LevelConfig levelConfig = gameConfig.Levels[levelIndex];
          if (levelConfig == null || levelConfig.Scene == null || levelConfig.Scene.Scene == null)
            continue;

          string sceneName = levelConfig.Scene.Scene.name;
          if (!processedSceneNames.Add(sceneName))
            continue;

          string leaderboardConfigPath = $"{_kLeaderboardConfigsFolderPath}/{sceneName}.lb";
          if (File.Exists(leaderboardConfigPath))
            continue;

          File.WriteAllText(leaderboardConfigPath, CreateConfigJson(sceneName));
          createdCount++;
        }
      }

      if (folderCreated || createdCount > 0)
        AssetDatabase.Refresh();

      Debug.Log($"Created {createdCount} missing leaderboard configuration file(s).");
    }

    private static string CreateConfigJson(string sceneName)
    {
      return
        "{\n" +
        "  \"$schema\": \"https://ugs-config-schemas.unity3d.com/v1/leaderboards.schema.json\",\n" +
        $"  \"Name\": \"{sceneName}\",\n" +
        "  \"SortOrder\": \"asc\",\n" +
        "  \"UpdateType\": \"keepBest\"\n" +
        "}\n";
    }
  }
}
