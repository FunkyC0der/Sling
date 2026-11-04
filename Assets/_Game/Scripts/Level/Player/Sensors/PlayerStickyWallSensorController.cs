using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Collision;
using Sling.Level.Elements.StickyWall;
using UnityEngine;

namespace Sling.Level.Player.Sensors
{
  /// <summary>Trigger overlap with a <see cref="StickyWall"/> → <see cref="PlayerModel.IsOnStickyWall"/>.</summary>
  public class PlayerStickyWallSensorController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly CollisionTriggerView _collisionTriggerView;
    private readonly HashSet<GameObject> _walls = new();

    public PlayerStickyWallSensorController(IControllerFactory controllerFactory,
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

      events.OnTriggerExit += OnTriggerExit;
      this.AddDisposableAction(() => events.OnTriggerExit -= OnTriggerExit);
    }

    private void OnTriggerEnter(Collider2D other)
    {
      if (!other.GetComponentInParent<StickyWall>())
        return;

      _walls.Add(other.gameObject);
      _model.IsOnStickyWall.Value = true;
    }

    private void OnTriggerExit(Collider2D other)
    {
      if (!other.GetComponentInParent<StickyWall>())
        return;

      _walls.Remove(other.gameObject);
      _model.IsOnStickyWall.Value = _walls.Count > 0;
    }
  }
}
