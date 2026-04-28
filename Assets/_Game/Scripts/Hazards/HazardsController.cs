using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Level;

namespace Sling.Hazards
{
    public class HazardsController : ControllerBase
    {
        private readonly List<HazardView> _hazardViews;
        private readonly LevelEvents _events;

        public HazardsController(IControllerFactory factory, List<HazardView> hazardViews, LevelEvents events) : base(factory)
        {
            _hazardViews = hazardViews;
            _events = events;
        }

        protected override void OnStart()
        {
            foreach (HazardView hazardView in _hazardViews)
            {
                hazardView.OnPlayerHit += _events.OnPlayerDied;
                AddDisposable(new DisposableToken(() => hazardView.OnPlayerHit -= _events.OnPlayerDied));
            }
        }
    }
}
