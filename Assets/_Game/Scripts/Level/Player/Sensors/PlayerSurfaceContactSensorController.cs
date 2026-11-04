using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Collision;
using UnityEngine;

namespace Sling.Level.Player.Sensors
{
  /// <summary>
  /// Any contact (collision or trigger, from any side) with the ground surface layers →
  /// <see cref="PlayerModel.IsTouchingSurface"/>. Refills air launches.
  /// </summary>
  public class PlayerSurfaceContactSensorController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly CollisionTriggerView _collisionTriggerView;
    private readonly HashSet<GameObject> _contacts = new();

    public PlayerSurfaceContactSensorController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerConfig config,
      CollisionTriggerView collisionTriggerView)
      : base(controllerFactory)
    {
      _model = model;
      _config = config;
      _collisionTriggerView = collisionTriggerView;
    }

    protected override void OnStart()
    {
      CollisionEvents events = _collisionTriggerView.Events;

      events.OnEnter += OnCollisionEnter;
      this.AddDisposableAction(() => events.OnEnter -= OnCollisionEnter);

      events.OnExit += OnCollisionExit;
      this.AddDisposableAction(() => events.OnExit -= OnCollisionExit);

      events.OnTriggerEnter += OnTriggerEnter;
      this.AddDisposableAction(() => events.OnTriggerEnter -= OnTriggerEnter);

      events.OnTriggerExit += OnTriggerExit;
      this.AddDisposableAction(() => events.OnTriggerExit -= OnTriggerExit);
    }

    private void OnCollisionEnter(Collision2D collision) =>
      Add(collision.collider.gameObject);

    private void OnCollisionExit(Collision2D collision) =>
      Remove(collision.collider.gameObject);

    private void OnTriggerEnter(Collider2D other) =>
      Add(other.gameObject);

    private void OnTriggerExit(Collider2D other) =>
      Remove(other.gameObject);

    private void Add(GameObject obj)
    {
      if (!_config.GroundSurfaceLayerMask.HasLayer(obj.layer))
        return;

      _contacts.Add(obj);
      _model.IsTouchingSurface.Value = true;
    }

    private void Remove(GameObject obj)
    {
      if (!_config.GroundSurfaceLayerMask.HasLayer(obj.layer))
        return;

      _contacts.Remove(obj);
      _model.IsTouchingSurface.Value = _contacts.Count > 0;
    }
  }
}
