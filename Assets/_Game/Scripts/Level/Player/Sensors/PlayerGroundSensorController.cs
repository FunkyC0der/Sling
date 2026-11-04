using Playtika.Controllers;
using Sling.Common.Extensions;
using UnityEngine;

namespace Sling.Level.Player.Sensors
{
  /// <summary>Feet trigger → <see cref="PlayerModel.IsGrounded"/>.</summary>
  public class PlayerGroundSensorController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerGroundedView _groundedView;

    public PlayerGroundSensorController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerGroundedView groundedView)
      : base(controllerFactory)
    {
      _model = model;
      _groundedView = groundedView;
    }

    protected override void OnStart()
    {
      _model.IsGrounded.Value = _groundedView.IsColliding();

      _groundedView.OnGrounded += OnGrounded;
      this.AddDisposableAction(() => _groundedView.OnGrounded -= OnGrounded);

      _groundedView.OnUngrounded += OnUngrounded;
      this.AddDisposableAction(() => _groundedView.OnUngrounded -= OnUngrounded);
    }

    private void OnGrounded(Collider2D other) =>
      _model.IsGrounded.Value = true;

    private void OnUngrounded(Collider2D other) =>
      _model.IsGrounded.Value = false;
  }
}
