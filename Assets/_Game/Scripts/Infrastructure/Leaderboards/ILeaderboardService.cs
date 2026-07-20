using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Infrastructure.Leaderboards
{
  public interface ILeaderboardService
  {
    UniTask<IReadOnlyList<LeaderboardPlayerScore>> GetTopPlayersForLevelAsync(
      string levelId,
      int topCount,
      CancellationToken cancellationToken);

    UniTask SetPlayerScoreAsync(
      string levelId,
      int deathCount,
      float timeInSeconds,
      CancellationToken cancellationToken);

    UniTask<int?> GetPlayerRankAsync(
      string levelId,
      CancellationToken cancellationToken);
  }
}
