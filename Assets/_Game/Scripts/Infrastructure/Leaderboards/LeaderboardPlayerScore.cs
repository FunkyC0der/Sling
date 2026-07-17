namespace Sling.Infrastructure.Leaderboards
{
  public class LeaderboardPlayerScore
  {
    public string PlayerName;
    public int Rank;
    public int DeathCount;
    public float TimeInSeconds;

    public LeaderboardPlayerScore(
      string playerName,
      int rank,
      int deathCount,
      float timeInSeconds)
    {
      PlayerName = playerName;
      Rank = rank;
      DeathCount = deathCount;
      TimeInSeconds = timeInSeconds;
    }
  }
}
