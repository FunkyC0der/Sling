using System;
using Sling.Level.StickyWall;

namespace Sling.Level
{
    public class LevelEvents
    {
        public Action OnPlayerDied;
        public Action OnFinishReached;
        public Action OnRestartRequested;
        
        public Action<StickyWallView> OnPlayerEnterStickyWall;
        public Action<StickyWallView> OnPlayerExitStickyWall;
    }
}
