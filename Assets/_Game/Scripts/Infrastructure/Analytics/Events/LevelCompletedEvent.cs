using Unity.Services.Analytics;

namespace Sling.Infrastructure.Analytics.Events
{
  public class LevelCompletedEvent : Event
  {
    public LevelCompletedEvent(int levelIndex, int playerDeathCount, float timeToCompleteInSeconds) 
      : base(AnalyticsNames.Events.kLevelCompleted)
    {
      SetParameter(AnalyticsNames.Parameters.kLevelNumber, levelIndex + 1);
      SetParameter(AnalyticsNames.Parameters.kPlayerDeathCount, playerDeathCount);
      SetParameter(AnalyticsNames.Parameters.kTimeToComplete, timeToCompleteInSeconds);
    }
  }
}
