using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;

namespace Sling.Infrastructure.Leaderboards
{
  public class LeaderboardService
  {
    public async UniTask<IReadOnlyList<LeaderboardPlayerScore>> GetTopPlayersForLevelAsync(
      string levelId,
      int topCount,
      CancellationToken cancellationToken)
    {
      if (topCount <= 0)
        throw new ArgumentOutOfRangeException(nameof(topCount), topCount, "Top count must be greater than zero.");

      var options = new GetScoresOptions
      {
        Offset = 0,
        Limit = topCount,
        IncludeMetadata = true
      };

      LeaderboardScoresPage scoresPage = await LeaderboardsService.Instance.GetScoresAsync(levelId, options)
        .AsUniTask()
        .AttachExternalCancellation(cancellationToken);

      var scores = new List<LeaderboardPlayerScore>(scoresPage.Results.Count);
      foreach (LeaderboardEntry entry in scoresPage.Results)
        scores.Add(ToPlayerScore(entry));

      return scores;
    }

    public async UniTask SetPlayerScoreAsync(
      string levelId,
      int deathCount,
      float timeInSeconds,
      CancellationToken cancellationToken)
    {
      long packedScore = PackScore(deathCount, timeInSeconds);
      var options = new AddPlayerScoreOptions
      {
        Metadata = new LeaderboardScoreMetadata
        {
          DeathCount = deathCount,
          TimeInSeconds = timeInSeconds
        }
      };

      await LeaderboardsService.Instance.AddPlayerScoreAsync(levelId, packedScore, options)
        .AsUniTask()
        .AttachExternalCancellation(cancellationToken);
    }

    public async UniTask<int?> GetPlayerRankAsync(
      string levelId,
      CancellationToken cancellationToken)
    {
      try
      {
        LeaderboardEntry entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(
            levelId,
            new GetPlayerScoreOptions())
          .AsUniTask()
          .AttachExternalCancellation(cancellationToken);

        return entry.Rank;
      }
      catch (LeaderboardsException exception)
        when (exception.Reason == LeaderboardsExceptionReason.EntryNotFound)
      {
        return null;
      }
    }

    private static long PackScore(int deathCount, float timeInSeconds)
    {
      if (deathCount < 0)
        throw new ArgumentOutOfRangeException(nameof(deathCount), deathCount, "Death count cannot be negative.");

      if (float.IsNaN(timeInSeconds) || float.IsInfinity(timeInSeconds) || timeInSeconds < 0f)
        throw new ArgumentOutOfRangeException(nameof(timeInSeconds), timeInSeconds,
          "Time must be finite and cannot be negative.");

      double roundedMilliseconds = Math.Round(timeInSeconds * 1000d, MidpointRounding.AwayFromZero);
      if (roundedMilliseconds > uint.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(timeInSeconds), timeInSeconds,
          "Time in milliseconds cannot exceed UInt32.MaxValue.");

      uint timeMilliseconds = (uint)roundedMilliseconds;
      return ((long)deathCount << 32) | timeMilliseconds;
    }

    private static LeaderboardPlayerScore ToPlayerScore(LeaderboardEntry entry)
    {
      if (!string.IsNullOrEmpty(entry.Metadata))
      {
        LeaderboardScoreMetadata metadata = JsonConvert.DeserializeObject<LeaderboardScoreMetadata>(entry.Metadata);
        if (metadata != null)
          return new LeaderboardPlayerScore(
            entry.PlayerName,
            entry.Rank,
            metadata.DeathCount,
            metadata.TimeInSeconds);
      }

      return DecodePlayerScore(entry);
    }

    private static LeaderboardPlayerScore DecodePlayerScore(LeaderboardEntry entry)
    {
      const double longExclusiveUpperBound = 9223372036854775808d;
      if (double.IsNaN(entry.Score) || double.IsInfinity(entry.Score) ||
          entry.Score < 0d || entry.Score >= longExclusiveUpperBound)
        throw new InvalidOperationException($"Leaderboard score '{entry.Score}' cannot be decoded.");

      long packedScore = (long)Math.Round(entry.Score, MidpointRounding.AwayFromZero);
      int deathCount = (int)(packedScore >> 32);
      uint timeMilliseconds = (uint)(packedScore & uint.MaxValue);

      return new LeaderboardPlayerScore(
        entry.PlayerName,
        entry.Rank,
        deathCount,
        timeMilliseconds / 1000f);
    }

    private class LeaderboardScoreMetadata
    {
      public int DeathCount;
      public float TimeInSeconds;
    }
  }
}
