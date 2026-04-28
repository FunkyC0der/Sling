using System.Collections.Generic;
using Playtika.Controllers;

namespace Sling.Level.StickyWall
{
    public class StickyWallsController : ControllerBase
    {
        private readonly IReadOnlyList<StickyWallView> _stickyWalls;
        private readonly LevelEvents _levelEvents;

        public StickyWallsController(IControllerFactory controllerFactory, IReadOnlyList<StickyWallView> stickyWalls, LevelEvents levelEvents) 
            : base(controllerFactory)
        {
            _stickyWalls = stickyWalls;
            _levelEvents = levelEvents;
        }

        protected override void OnStart()
        {
            foreach (StickyWallView stickyWall in _stickyWalls)
            {
                stickyWall.OnPlayerEnter += _levelEvents.OnPlayerEnterStickyWall;
                stickyWall.OnPlayerExit += _levelEvents.OnPlayerExitStickyWall;
            }
        }

        protected override void OnStop()
        {
            foreach (StickyWallView stickyWall in _stickyWalls)
            {
                stickyWall.OnPlayerEnter -= _levelEvents.OnPlayerEnterStickyWall;
                stickyWall.OnPlayerExit -= _levelEvents.OnPlayerExitStickyWall;
            }
        }
    }
}