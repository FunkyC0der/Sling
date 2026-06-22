using Playtika.Controllers;
using Sling.Infrastructure;
using UnityEngine;

namespace Sling.Level.Session
{
  public class LevelTimeController : ControllerBase
  {
    private readonly LevelModel _levelModel;
    private readonly UpdateEvents _updateEvents;

    public LevelTimeController(IControllerFactory controllerFactory, LevelModel levelModel, UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _levelModel = levelModel;
      _updateEvents = updateEvents;
    }

    protected override void OnStart() => 
      _updateEvents.OnUpdate += Update;

    protected override void OnStop() => 
      _updateEvents.OnUpdate -= Update;

    private void Update() => 
      _levelModel.ElapsedTimeInSeconds += Time.deltaTime;
  }
}