using Playtika.Controllers;

namespace Sling.Level.Session
{
  public class PlayerDeathCountController : ControllerBase
  {
    private readonly LevelEvents _levelEvents;
    private readonly LevelModel _levelModel;

    public PlayerDeathCountController(IControllerFactory controllerFactory,
      LevelEvents levelEvents,
      LevelModel levelModel) 
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
      _levelModel = levelModel;
    }

    protected override void OnStart() => 
      _levelEvents.OnPlayerDied += () => ++_levelModel.PlayerDeathCount.Value;
  }
}
