using System;
using Playtika.Controllers;
using Sling.Player.Views;
using UnityEngine;

namespace Sling.Player
{
  public class LaunchController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerView _playerView;
    private readonly LaunchTrajectoryView _launchTrajectoryView;
    private readonly Func<float, Vector3> _trajectorySamplePosFunc;

    private Vector2 _preLaunchStartPos;
    private Vector2 _startVelocity;
    private Vector2 _launchForce;

    public LaunchController(IControllerFactory controllerFactory,
      PlayerInputView inputView,
      PlayerView playerView,
      LaunchTrajectoryView launchTrajectoryView)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _playerView = playerView;
      _launchTrajectoryView = launchTrajectoryView;
      _trajectorySamplePosFunc = SamplePosition;
    }

    protected override void OnStart()
    {
      _inputView.OnPreLaunchStart += OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate += OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop += OnPreLaunchStop;
    }

    protected override void OnStop()
    {
      _inputView.OnPreLaunchStart -= OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate -= OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop -= OnPreLaunchStop;
    }

    private void OnPreLaunchStart(Vector2 worldPos) =>
      _preLaunchStartPos = worldPos;

    private void OnPreLaunchUpdate(Vector2 worldPos)
    {
      Vector2 launchVector = _preLaunchStartPos - worldPos;
      launchVector = Vector2.ClampMagnitude(launchVector, _playerView.Config.MaxDragDistance);
      
      _launchForce = launchVector * _playerView.Config.LaunchForceMultiplier;
      _startVelocity = _launchForce / _playerView.Mass;

      _launchTrajectoryView.Show(_trajectorySamplePosFunc);
    }

    private void OnPreLaunchStop(Vector2 worldPos)
    {
      _launchTrajectoryView.Hide();
      _playerView.Launch(_launchForce);
    }

    private Vector3 SamplePosition(float t) =>
      _startVelocity * t + Physics2D.gravity * (0.5f * t * t);
  }
}
