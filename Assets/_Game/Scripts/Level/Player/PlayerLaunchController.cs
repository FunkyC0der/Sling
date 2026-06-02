using System;
using Playtika.Controllers;
using Sling.Common;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerLaunchController : ControllerBase<PlayerLaunchController.Context>
  {
    public class Context
    {
      public readonly Observable<bool> IsInAir;

      public Context(Observable<bool> isInAir)
      {
        IsInAir = isInAir;
      }
    }
    
    private readonly PlayerInputView _inputView;
    private readonly PlayerView _view;
    private readonly LaunchTrajectoryView _launchTrajectoryView;
    private readonly Func<float, Vector3> _trajectorySamplePosFunc;

    private Vector2 _preLaunchStartPos;
    private Vector2 _startVelocity;
    private bool _isFirstLaunch;
    private bool _isInLaunchState;
    private int _remainingLaunches;

    public PlayerLaunchController(IControllerFactory controllerFactory,
      PlayerInputView inputView,
      PlayerView view,
      LaunchTrajectoryView launchTrajectoryView)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _view = view;
      _launchTrajectoryView = launchTrajectoryView;
      _trajectorySamplePosFunc = SamplePosition;
    }

    protected override void OnStart()
    {
      _inputView.OnPreLaunchStart += OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate += OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop += OnPreLaunchStop;

      Args.IsInAir.OnValueChanged += OnIsInAirChanged;
      
      _isFirstLaunch = true;
      _view.SetPhysicsEnabled(false);

      ResetRemainingLaunches();
    }

    protected override void OnStop()
    {
      Args.IsInAir.OnValueChanged -= OnIsInAirChanged;
      
      _inputView.OnPreLaunchStart -= OnPreLaunchStart;
      _inputView.OnPreLaunchUpdate -= OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop -= OnPreLaunchStop;
    }

    private void OnPreLaunchStart(Vector2 worldPos)
    {
      if (!_isFirstLaunch && Args.IsInAir.Value && _remainingLaunches <= 0)
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

      _startVelocity = launchVector * _view.Config.LaunchForceMultiplier;

      float forceFraction = launchVector.magnitude / _view.Config.MaxDragDistance;
      float totalTime = _view.Config.TrajectoryHintDuration * forceFraction;
      _launchTrajectoryView.Show(totalTime, _trajectorySamplePosFunc);
    }

    private void OnPreLaunchStop(Vector2 worldPos)
    {
      if (!_isInLaunchState || !(_startVelocity.sqrMagnitude > 0))
        return;

      _isInLaunchState = false;
      _remainingLaunches = Mathf.Max(0, _remainingLaunches - 1);
      
      _launchTrajectoryView.Hide();

      if(_isFirstLaunch)
      {
        _isFirstLaunch = false;
        _view.SetPhysicsEnabled(true);
      }

      _view.Launch(_startVelocity);
    }

    private Vector3 SamplePosition(float t) =>
      _startVelocity * t + Physics2D.gravity * (0.5f * t * t);

    private void OnIsInAirChanged(bool oldValue, bool newValue)
    {
      if (!newValue)
        ResetRemainingLaunches();
    }

    private void ResetRemainingLaunches() => 
      _remainingLaunches = 1 + _view.Config.MaxAirLaunches;
  }
}
