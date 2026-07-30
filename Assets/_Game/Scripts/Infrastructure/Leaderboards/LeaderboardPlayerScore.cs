namespace Sling.Infrastructure.Leaderboards
{
  public class LeaderboardPlayerScore
  {
    public string PlayerName;
    public int Rank;
    public int DeathCount;
    public float TimeInSeconds;
    public int PlayerLaunchCount;

    public LeaderboardPlayerScore(
      string playerName,
      int rank,
      int deathCount,
      float timeInSeconds,
      int playerLaunchCount)
    {
      PlayerName = playerName;
      Rank = rank;
      DeathCount = deathCount;
      TimeInSeconds = timeInSeconds;
      PlayerLaunchCount = playerLaunchCount;
    }
  }
}
