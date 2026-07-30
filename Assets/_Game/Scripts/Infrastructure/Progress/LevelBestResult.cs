namespace Sling.Infrastructure.Progress
{
  public class LevelBestResult
  {
    public int DeathCount;
    public float TimeInSeconds;
    public int PlayerLaunchCount;

    public LevelBestResult(int deathCount, float timeInSeconds, int playerLaunchCount)
    {
      DeathCount = deathCount;
      TimeInSeconds = timeInSeconds;
      PlayerLaunchCount = playerLaunchCount;
    }
  }
}
