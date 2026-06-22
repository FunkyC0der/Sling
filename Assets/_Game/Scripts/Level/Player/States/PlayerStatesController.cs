using Playtika.Controllers;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player.States
{
  public class PlayerStatesController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerGroundedView _groundedView;
    private readonly LevelEvents _levelEvents;

    public PlayerStatesController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerGroundedView groundedView, 
      LevelEvents levelEvents)
      : base(controllerFactory)
    {
      _model = model;
      _groundedView = groundedView;
      _levelEvents = levelEvents;
    }

    protected override void OnStart()
    {
      _groundedView.OnGrounded += OnGrounded;
      _groundedView.OnUngrounded += OnUngrounded;

      _levelEvents.OnLevelCompleted += OnLevelCompleted;
    }

    protected override void OnStop()
    {
      _groundedView.OnGrounded -= OnGrounded;
      _groundedView.OnUngrounded -= OnUngrounded;
      
      _levelEvents.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnGrounded(Collider2D other) => 
      _model.IsGrounded.Value = true;

    private void OnUngrounded(Collider2D other) => 
      _model.IsGrounded.Value = false;

    private void OnLevelCompleted() => 
      _model.IsWin.Value = true;
  }
}