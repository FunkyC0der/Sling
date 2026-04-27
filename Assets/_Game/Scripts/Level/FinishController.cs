using Playtika.Controllers;

namespace Sling.Level
{
    public class FinishController : ControllerBase
    {
        private readonly FinishView _view;
        private readonly LevelEvents _events;

        public FinishController(IControllerFactory factory, FinishView view, LevelEvents events) : base(factory)
        {
            _view = view;
            _events = events;
        }

        protected override void OnStart()
        {
            AddDisposable(new DisposableToken(() => _view.OnPlayerReachedFinish -= OnFinishReached));
            _view.OnPlayerReachedFinish += OnFinishReached;
        }

        private void OnFinishReached()
        {
            _events.RaiseFinishReached();
        }
    }
}
