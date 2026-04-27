using Playtika.Controllers;
using Sling.Player.Trajectory;
using UnityEngine;

namespace Sling.Player
{
    public class PlayerController : ControllerBase
    {
        private readonly PlayerView _view;

        private bool _isDragging;
        private PlayerInputEvents _events;

        public PlayerController(IControllerFactory factory, PlayerView view) : base(factory) =>
            _view = view;

        protected override void OnStart()
        {
            _events = new PlayerInputEvents();
            _view.Bind(_events);

            AddDisposable(new DisposableToken(() =>
            {
                _events.OnPointerDown -= OnPointerDown;
                _view.Unbind();
            }));

            _events.OnPointerDown += OnPointerDown;
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
    }
}
