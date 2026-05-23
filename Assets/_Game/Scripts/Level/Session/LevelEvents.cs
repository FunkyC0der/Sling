using System;

namespace Sling.Level.Session
{
  public class LevelEvents
  {
    public Action OnPlayerDied;
    public Action OnLevelCompleted;
    public Action OnRestartRequested;
    public Action OnMenuRequested;
  }
}