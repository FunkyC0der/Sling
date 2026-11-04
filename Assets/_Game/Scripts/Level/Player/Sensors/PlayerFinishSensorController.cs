using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Collision;
using Sling.Level.Finish;
using UnityEngine;

namespace Sling.Level.Player.Sensors
{
  /// <summary>Finish zone overlap → <see cref="PlayerModel.RaiseFinishReached"/>.</summary>
  public class PlayerFinishSensorController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly CollisionTriggerView _collisionTriggerView;

    public PlayerFinishSensorController(IControllerFactory controllerFactory,
      PlayerModel model,
      CollisionTriggerView collisionTriggerView)
      : base(controllerFactory)
    {
      _model = model;
      _collisionTriggerView = collisionTriggerView;
    }

    protected override void OnStart()
    {
      CollisionEvents events = _collisionTriggerView.Events;

      events.OnTriggerEnter += OnTriggerEnter;
      this.AddDisposableAction(() => events.OnTriggerEnter -= OnTriggerEnter);
    }

    private void OnTriggerEnter(Collider2D other)
    {
      if (other.GetComponent<FinishZoneView>())
        _model.RaiseFinishReached();
    }
  }
}
