using System;

namespace Sling.Level
{
  public class LevelEvents
  {
    public Action OnPlayerDied;
    public Action OnFinishReached;
    public Action OnRestartRequested;
  }
}