namespace Sling.Infrastructure.Progress
{
  public class LevelBestResult
  {
    public int DeathCount;
    public float TimeInSeconds;

    public LevelBestResult(int deathCount, float timeInSeconds)
    {
      DeathCount = deathCount;
      TimeInSeconds = timeInSeconds;
    }
  }
}
