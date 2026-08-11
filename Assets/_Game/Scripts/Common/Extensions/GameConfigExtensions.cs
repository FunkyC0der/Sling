using System.Collections.Generic;
using Sling.Levels;

namespace Sling.Common.Extensions
{
  public static class GameConfigExtensions
  {
    public static LevelConfig GetLevel(this GameConfig config, int worldIndex, int levelIndex) =>
      config.Worlds[worldIndex].Levels[levelIndex];

    public static bool TryGetNextLevel(this GameConfig config, int worldIndex, int levelIndex,
      out int nextWorldIndex, out int nextLevelIndex)
    {
      WorldConfig currentWorld = config.Worlds[worldIndex];

      if (levelIndex + 1 < currentWorld.Levels.Count)
      {
        nextWorldIndex = worldIndex;
        nextLevelIndex = levelIndex + 1;
        return true;
      }

      if (worldIndex + 1 < config.Worlds.Count)
      {
        nextWorldIndex = worldIndex + 1;
        nextLevelIndex = 0;
        return true;
      }

      nextWorldIndex = worldIndex;
      nextLevelIndex = levelIndex;
      return false;
    }

    public static bool TryFindLevel(this GameConfig config, string sceneName, out int worldIndex, out int levelIndex)
    {
      for (int w = 0; w < config.Worlds.Count; w++)
      {
        List<LevelConfig> levels = config.Worlds[w].Levels;
        int foundLevelIndex = levels.FindIndex(level => level.Scene.SceneName == sceneName);
        if (foundLevelIndex > -1)
        {
          worldIndex = w;
          levelIndex = foundLevelIndex;
          return true;
        }
      }

      worldIndex = -1;
      levelIndex = -1;
      return false;
    }
  }
}
