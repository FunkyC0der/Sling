using System;
using Sling.Level.StickyWall;

namespace Sling.Level
{
    public class LevelEvents
    {
        public event Action OnPlayerDied;
        public event Action OnFinishReached;
        public event Action OnRestartRequested;
        
        public Action<StickyWallView> OnPlayerEnterStickyWall;
        public Action<StickyWallView> OnPlayerExitStickyWall;

        public void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public void RaiseFinishReached() => OnFinishReached?.Invoke();
        public void RaiseRestartRequested() => OnRestartRequested?.Invoke();
    }
}
