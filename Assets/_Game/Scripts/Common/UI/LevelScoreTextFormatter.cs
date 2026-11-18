using System;

namespace Sling.Common.UI
{
  public static class LevelScoreTextFormatter
  {
    public static string FormatLevelName(int levelIndex) =>
      $"LVL {levelIndex + 1}";

    public static string FormatPlayerDeathCount(int playerDeathCount) =>
      playerDeathCount.ToString();

    public static string FormatPlayerLaunchCount(int playerLaunchCount) =>
      playerLaunchCount.ToString();

    public static string FormatElapsedTime(float elapsedTimeInSeconds) =>
      TimeSpan.FromSeconds(elapsedTimeInSeconds).ToString(@"mm\:ss\.ff");
  }
}
