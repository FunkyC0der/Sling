using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Infrastructure.Leaderboards
{
  public class DummyLeaderboardService : ILeaderboardService
  {
    public UniTask<IReadOnlyList<LeaderboardPlayerScore>> GetTopPlayersForLevelAsync(
      string levelId,
      int topCount,
      CancellationToken cancellationToken) =>
      UniTask.FromResult<IReadOnlyList<LeaderboardPlayerScore>>(Array.Empty<LeaderboardPlayerScore>());

    public UniTask SetPlayerScoreAsync(
      string levelId,
      int deathCount,
      float timeInSeconds,
      CancellationToken cancellationToken) =>
      UniTask.CompletedTask;

    public UniTask<int?> GetPlayerRankAsync(
      string levelId,
      CancellationToken cancellationToken) =>
      UniTask.FromResult<int?>(null);
  }
}
