using Playtika.Controllers;

public class HazardsController : ControllerBase<LevelSceneContext>
{
    public HazardsController(IControllerFactory factory) : base(factory) { }

    protected override void OnStart()
    {
        foreach (var hazard in Args.Hazards)
        {
            var h = hazard;
            h.OnPlayerHit += OnHazardHit;
            AddDisposable(new DisposableToken(() => h.OnPlayerHit -= OnHazardHit));
        }
    }

    private void OnHazardHit()
    {
        Args.LevelEvents.RaisePlayerDied();
    }
}
