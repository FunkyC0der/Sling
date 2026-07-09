namespace Sling.Infrastructure.Progress
{
  public static class LevelBestResultComparer
  {
    public static bool IsBetter(LevelBestResult candidate, LevelBestResult current)
    {
      if (candidate.DeathCount != current.DeathCount)
        return candidate.DeathCount < current.DeathCount;

      return candidate.TimeInSeconds < current.TimeInSeconds;
    }
  }
}
