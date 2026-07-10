using System;

namespace Sling.Common.UI
{
  public static class LevelScoreTextFormatter
  {
    public static string FormatPlayerDeathCount(int playerDeathCount) =>
      $"{playerDeathCount} DEATHs";

    public static string FormatElapsedTime(float elapsedTimeInSeconds) =>
      $"TIME {TimeSpan.FromSeconds(elapsedTimeInSeconds):mm\\:ss\\.ff}";
  }
}
