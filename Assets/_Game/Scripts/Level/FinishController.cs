using Playtika.Controllers;

namespace Sling.Level
{
    public class FinishController : ControllerBase
    {
        private readonly FinishView _finishView;
        private readonly LevelEvents _events;

        public FinishController(IControllerFactory factory, FinishView finishView, LevelEvents events)
            : base(factory)
        {
            _finishView = finishView;
            _events = events;
        }

        protected override void OnStart() => 
            _finishView.OnReached += _events.OnFinishReached;

        protected override void OnStop() => 
            _finishView.OnReached -= _events.OnFinishReached;
    }
}
