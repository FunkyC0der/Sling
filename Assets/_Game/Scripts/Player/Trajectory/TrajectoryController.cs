using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Player.Trajectory
{
  public class TrajectoryController : ControllerWithResultBase<TrajectoryArgs, Vector2>
  {
    private readonly TrajectoryView _trajectoryView;
    private readonly PlayerConfig _playerConfig;

    private Vector2 _dragStart;
    private Vector2 _startVelocity;
    private Vector2 _lastForce;
    private Func<float, Vector3> _sampleFunc;

    public TrajectoryController(
      IControllerFactory factory,
      TrajectoryView trajectoryView,
      PlayerConfig playerConfig)
      : base(factory)
    {
      _trajectoryView = trajectoryView;
      _playerConfig = playerConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _dragStart = Args.DragStartPos;
      _sampleFunc = SamplePosition;

      AddDisposable(new DisposableToken(() =>
      {
        Args.Events.OnPointerDragged -= Recompute;
        Args.Events.OnPointerUp -= OnPointerUp;
      }));

      Args.Events.OnPointerDragged += Recompute;
      Args.Events.OnPointerUp += OnPointerUp;

      await UniTask.Never(cancellationToken);
    }

    private void Recompute(Vector2 currentWorldPos)
    {
      Vector2 dragVector = Vector2.ClampMagnitude(_dragStart - currentWorldPos, _playerConfig.MaxDragDistance);
      _lastForce = dragVector * _playerConfig.LaunchForceMultiplier;
      _startVelocity = _lastForce / Args.Mass;
      _trajectoryView.Show(_sampleFunc);
    }

    private void OnPointerUp(Vector2 _)
    {
      _trajectoryView.Hide();
      Complete(_lastForce);
    }

    private Vector3 SamplePosition(float t)
    {
      return _startVelocity * t + Physics2D.gravity * (0.5f * t * t);
    }
  }
}