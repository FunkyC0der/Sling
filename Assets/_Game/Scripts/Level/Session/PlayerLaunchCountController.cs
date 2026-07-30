using Playtika.Controllers;
using Sling.Common.Extensions;

namespace Sling.Level.Session
{
  public class PlayerLaunchCountController : ControllerBase
  {
    private readonly LevelEvents _levelEvents;
    private readonly LevelModel _levelModel;

    public PlayerLaunchCountController(
      IControllerFactory controllerFactory,
      LevelEvents levelEvents,
      LevelModel levelModel)
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
      _levelModel = levelModel;
    }

    protected override void OnStart()
    {
      _levelEvents.OnPlayerLaunched += OnPlayerLaunched;
      this.AddDisposableAction(() => _levelEvents.OnPlayerLaunched -= OnPlayerLaunched);
    }

    private void OnPlayerLaunched() =>
      ++_levelModel.PlayerLaunchCount.Value;
  }
}
