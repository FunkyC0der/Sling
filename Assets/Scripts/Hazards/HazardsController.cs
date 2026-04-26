using Playtika.Controllers;

public class HazardsController : ControllerBase
{
    private readonly HazardView[] _hazards;
    private readonly LevelEvents _events;

    public HazardsController(IControllerFactory factory, HazardView[] hazards, LevelEvents events) : base(factory)
    {
        _hazards = hazards;
        _events = events;
    }

    protected override void OnStart()
    {
        foreach (var hazard in _hazards)
        {
            var h = hazard;
            h.OnPlayerHit += OnHazardHit;
            AddDisposable(new DisposableToken(() => h.OnPlayerHit -= OnHazardHit));
        }
    }

    private void OnHazardHit()
    {
        _events.RaisePlayerDied();
    }
}
