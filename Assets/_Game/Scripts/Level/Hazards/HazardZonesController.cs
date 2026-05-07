using System.Collections.Generic;
using Playtika.Controllers;

namespace Sling.Level.Hazards
{
  public class HazardZonesController : ControllerBase
  {
    private readonly LevelEvents _levelEvents;
    private readonly IReadOnlyList<HazardZoneView> _zones;

    public HazardZonesController(IControllerFactory controllerFactory,
      LevelEvents levelEvents,
      HazardZoneView[] zones) : base(controllerFactory)
    {
      _levelEvents = levelEvents;
      _zones = zones;
    }

    protected override void OnStart()
    {
      foreach (HazardZoneView zone in _zones) 
        zone.OnEnter += _levelEvents.OnPlayerDied;
    }

    protected override void OnStop()
    {
      foreach (HazardZoneView zone in _zones)
        zone.OnEnter -= _levelEvents.OnPlayerDied;
    }
  }
}