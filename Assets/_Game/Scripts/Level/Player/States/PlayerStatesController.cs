using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player.States
{
  public class PlayerStatesController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerGroundedView _groundedView;

    public PlayerStatesController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerGroundedView groundedView)
      : base(controllerFactory)
    {
      _model = model;
      _groundedView = groundedView;
    }

    protected override void OnStart()
    {
      _groundedView.OnGrounded += OnGrounded;
      _groundedView.OnUngrounded += OnUngrounded;
    }

    protected override void OnStop()
    {
      _groundedView.OnGrounded -= OnGrounded;
      _groundedView.OnUngrounded -= OnUngrounded;
    }

    private void OnGrounded(Collider2D other) => 
      _model.IsGrounded.Value = true;

    private void OnUngrounded(Collider2D other) => 
      _model.IsGrounded.Value = false;
  }
}