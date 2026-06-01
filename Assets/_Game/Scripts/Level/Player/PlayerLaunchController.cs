using System;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerLaunchController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerView _view;
    private readonly PlayerIsInAirView _isInAirView;
    private readonly LaunchTrajectoryView _launchTrajectoryView;
    private readonly Func<float, Vector3> _trajectorySamplePosFunc;

    private Vector2 _preLaunchStartPos;
    private Vector2 _startVelocity;
    private Vector2 _launchForce;
    private bool _isFirstLaunch;
    private bool _isInLaunchState;
    private int _remainingAirLaunches = 1;

    public PlayerLaunchController(IControllerFactory controllerFactory,
      PlayerInputView inputView,
      PlayerView view,
      PlayerIsInAirView isInAirView,
      LaunchTrajectoryView launchTrajectoryView)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _view = view;
      _launchTrajectoryView = launchTrajectoryView;
      _isInAirView = isInAirView;
      _trajectorySamplePosFunc = SamplePosition;
    }

    protected override void OnStart()
    {
      _inputView.OnPreLaunchStart += OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate += OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop += OnPreLaunchStop;

      _isInAirView.OnStateChanged += OnIsInAirChanged;
      
      _isFirstLaunch = true;
      _view.SetPhysicsEnabled(false);
    }

    protected override void OnStop()
    {
      _isInAirView.OnStateChanged -= OnIsInAirChanged;
      
      _inputView.OnPreLaunchStart -= OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate -= OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop -= OnPreLaunchStop;
    }

    private void OnPreLaunchStart(Vector2 worldPos)
    {
      if (!_isFirstLaunch && _isInAirView.IsInAir && _remainingAirLaunches <= 0)
        return;

      _isInLaunchState = true;
      _preLaunchStartPos = worldPos;
    }

    private void OnPreLaunchUpdate(Vector2 worldPos)
    {
      if (!_isInLaunchState)
        return;
      
      Vector2 launchVector = _preLaunchStartPos - worldPos;
      launchVector = Vector2.ClampMagnitude(launchVector, _view.Config.MaxDragDistance);

      _launchForce = launchVector * _view.Config.LaunchForceMultiplier;
      _startVelocity = _launchForce / _view.Mass;

      float forceFraction = launchVector.magnitude / _view.Config.MaxDragDistance;
      float totalTime = _view.Config.TrajectoryHintDuration * forceFraction;
      _launchTrajectoryView.Show(totalTime, _trajectorySamplePosFunc);
    }

    private void OnPreLaunchStop(Vector2 worldPos)
    {
      if (!_isInLaunchState)
        return;

      _isInLaunchState = false;
      
      if(_isInAirView.IsInAir)
        _remainingAirLaunches = Mathf.Max(0, _remainingAirLaunches - 1);
      
      _launchTrajectoryView.Hide();

      if(_isFirstLaunch)
      {
        _isFirstLaunch = false;
        _view.SetPhysicsEnabled(true);
      }

      _view.Launch(_launchForce);
    }

    private Vector3 SamplePosition(float t) =>
      _startVelocity * t + Physics2D.gravity * (0.5f * t * t);

    private void OnIsInAirChanged()
    {
      if (!_isInAirView.IsInAir)
        _remainingAirLaunches = _view.Config.MaxAirLaunches;
    }
  }
}
