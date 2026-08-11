using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics.Events
{
  public class LevelCompletedEvent : Event
  {
    public LevelCompletedEvent(
      int worldIndex,
      int levelIndex,
      int playerDeathCount,
      int playerLaunchCount,
      float timeToCompleteInSeconds)
      : base(AnalyticsNames.Events.kLevelCompleted)
    {
      SetParameter(AnalyticsNames.Parameters.kWorldNumber, worldIndex + 1);
      SetParameter(AnalyticsNames.Parameters.kLevelNumber, levelIndex + 1);
      SetParameter(AnalyticsNames.Parameters.kPlayerDeathCount, playerDeathCount);
      SetParameter(AnalyticsNames.Parameters.kPlayerLaunchCount, playerLaunchCount);
      SetParameter(AnalyticsNames.Parameters.kTimeToComplete, timeToCompleteInSeconds);
    }
  }
}
