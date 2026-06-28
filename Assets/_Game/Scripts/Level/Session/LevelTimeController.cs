using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using UnityEngine;

namespace Sling.Level.Session
{
  public class LevelTimeController : ControllerBase
  {
    private readonly LevelModel _levelModel;
    private readonly UpdateEvents _updateEvents;
    private readonly LevelEvents _levelEvents;

    public LevelTimeController(
      IControllerFactory controllerFactory,
      LevelModel levelModel,
      UpdateEvents updateEvents,
      LevelEvents levelEvents)
      : base(controllerFactory)
    {
      _levelModel = levelModel;
      _updateEvents = updateEvents;
      _levelEvents = levelEvents;
    }

    protected override void OnStart()
    {
      _levelEvents.OnPlayerLaunched += OnPlayerFirstLaunch;
      this.AddDisposableAction(() => _levelEvents.OnPlayerLaunched -= OnPlayerFirstLaunch);
    }

    private void OnPlayerFirstLaunch()
    {
      _levelEvents.OnPlayerLaunched -= OnPlayerFirstLaunch;
      
      _updateEvents.OnUpdate += Update;
      this.AddDisposableAction(() => _updateEvents.OnUpdate -= Update);
    }

    private void Update() => 
      _levelModel.ElapsedTimeInSeconds += Time.deltaTime;
  }
}
