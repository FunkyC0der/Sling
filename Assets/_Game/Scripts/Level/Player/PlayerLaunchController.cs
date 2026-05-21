using System;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerLaunchController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerView _playerView;
    private readonly LaunchTrajectoryView _launchTrajectoryView;
    private readonly Func<float, Vector3> _trajectorySamplePosFunc;

    private Vector2 _preLaunchStartPos;
    private Vector2 _startVelocity;
    private Vector2 _launchForce;
    private bool _isFirstLaunch;

    public PlayerLaunchController(IControllerFactory controllerFactory,
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

      _isFirstLaunch = true;
      _playerView.SetPhysicsEnabled(false);
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

      float forceFraction = launchVector.magnitude / _playerView.Config.MaxDragDistance;
      float totalTime = _playerView.Config.TrajectoryHintDuration * forceFraction;
      _launchTrajectoryView.Show(totalTime, _trajectorySamplePosFunc);
    }

    private void OnPreLaunchStop(Vector2 worldPos)
    {
      _launchTrajectoryView.Hide();

      if(_isFirstLaunch)
      {
        _isFirstLaunch = false;
        _playerView.SetPhysicsEnabled(true);
      }

      _playerView.Launch(_launchForce);
    }

    private Vector3 SamplePosition(float t) =>
      _startVelocity * t + Physics2D.gravity * (0.5f * t * t);
  }
}
