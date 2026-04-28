using Playtika.Controllers;
using Sling.Level;
using Sling.Level.StickyWall;
using Sling.Player.Trajectory;
using UnityEngine;

namespace Sling.Player
{
    public class PlayerController : ControllerBase
    {
        private readonly PlayerView _view;
        private readonly LevelEvents _levelEvents;

        private bool _isDragging;
        private PlayerInputEvents _events;

        public PlayerController(IControllerFactory factory, PlayerView view, LevelEvents levelEvents)
            : base(factory)
        {
            _view = view;
            _levelEvents = levelEvents;
        }

        protected override void OnStart()
        {
            _events = new PlayerInputEvents();
            _events.OnPointerDown += OnPointerDown;

            _view.Bind(_events);
            
            _levelEvents.OnPlayerEnterStickyWall += OnPlayerEnterStickyWall;
            _levelEvents.OnPlayerExitStickyWall += OnPlayerExitStickyWall;
        }

        protected override void OnStop()
        {
            _events.OnPointerDown -= OnPointerDown;
            _view.Unbind();
        }

        private async void OnPointerDown(Vector2 worldPos)
        {
            if (_isDragging) return;
            _isDragging = true;

            try
            {
                var args = new TrajectoryArgs(worldPos, _view.Mass, _events);
                
                Vector2 force = await ExecuteAndWaitResultAsync<TrajectoryController, TrajectoryArgs, Vector2>(
                    args, CancellationToken);
                
                _view.Launch(force);
            }
            finally
            {
                _isDragging = false;
            }
        }

        private void OnPlayerEnterStickyWall(StickyWallView stickyWall) => 
            _view.SetMaxFallSpeed(stickyWall.Config.MaxFallSpeed);

        private void OnPlayerExitStickyWall(StickyWallView stickyWall) => 
            _view.SetMaxFallSpeed(float.PositiveInfinity);
    }
}
