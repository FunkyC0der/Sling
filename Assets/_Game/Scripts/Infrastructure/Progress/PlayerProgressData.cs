using System.Collections.Generic;

namespace Sling.Infrastructure.Progress
{
  public class PlayerProgressData
  {
    public Dictionary<string, LevelBestResult> LevelBestResults = new();

    public void EnsureInitialized() =>
      LevelBestResults ??= new Dictionary<string, LevelBestResult>();
  }
}
