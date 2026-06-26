using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Collision;
using Sling.Level.Finish;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player.States
{
  public class PlayerStatesController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerGroundedView _groundedView;
    private readonly LevelEvents _levelEvents;
    private readonly CollisionTriggerView _collisionTriggerView;

    public PlayerStatesController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerGroundedView groundedView, 
      LevelEvents levelEvents, 
      CollisionTriggerView collisionTriggerView)
      : base(controllerFactory)
    {
      _model = model;
      _groundedView = groundedView;
      _levelEvents = levelEvents;
      _collisionTriggerView = collisionTriggerView;
    }

    protected override void OnStart()
    {
      _model.IsGrounded.Value = _groundedView.IsColliding();
      _groundedView.OnGrounded += OnGrounded;
      this.AddDisposableAction(() => _groundedView.OnGrounded -= OnGrounded);
      
      _groundedView.OnUngrounded += OnUngrounded;
      this.AddDisposableAction(() => _groundedView.OnUngrounded -= OnUngrounded);

      _collisionTriggerView.Events.OnTriggerEnter += OnTriggerEnter;
      this.AddDisposableAction(() => _collisionTriggerView.Events.OnTriggerEnter -= OnTriggerEnter);
    }

    private void OnGrounded(Collider2D other) => 
      _model.IsGrounded.Value = true;

    private void OnUngrounded(Collider2D other) => 
      _model.IsGrounded.Value = false;

    private void OnTriggerEnter(Collider2D other)
    {
      if (other.GetComponent<FinishZoneView>())
      {
        _model.IsWin.Value = true;
        _levelEvents.OnLevelCompleted?.Invoke();
      }
    }
  }
}