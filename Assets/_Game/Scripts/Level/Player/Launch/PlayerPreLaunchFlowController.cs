using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player.Launch
{
  public class PlayerPreLaunchFlowController : ControllerWithResultBase<Vector2, Vector2>
  {
    private readonly PlayerConfig _config;
    private readonly PlayerLaunchView _launchView;
    private readonly PlayerInputView _inputView;
    private readonly PlayerLaunchView.SamplePositionFunc _samplePositionFunc;

    private Vector2 _launchVelocity;

    public PlayerPreLaunchFlowController(IControllerFactory controllerFactory,
      PlayerConfig config,
      PlayerInputView inputView,
      PlayerLaunchView launchView)
      : base(controllerFactory)
    {
      _config = config;
      _launchView = launchView;
      _inputView = inputView;

      _samplePositionFunc = SamplePosition;
    }

    protected override void OnStart()
    {
      _inputView.OnPreLaunchUpdate += OnPreLaunchUpdate;
      _inputView.OnPreLaunchStop += OnPreLaunchStop;  
    }

    protected override void OnStop()
    {
      _inputView.OnPreLaunchUpdate -= OnPreLaunchUpdate;  
      _inputView.OnPreLaunchStop -= OnPreLaunchStop;  
      
      _launchView.HideHint();
    }

    private void OnPreLaunchUpdate(Vector2 worldPos)
    {
      Vector2 launchVector = Args - worldPos;
      launchVector = Vector2.ClampMagnitude(launchVector, _config.MaxDragDistance);

      _launchVelocity = launchVector * _config.LaunchForceMultiplier;

      float forceFraction = launchVector.magnitude / _config.MaxDragDistance;
      float totalTime = _config.TrajectoryHintDuration * forceFraction;
      _launchView.ShowHint(totalTime, _samplePositionFunc);
    }

    private void OnPreLaunchStop(Vector2 worldPos) => 
      Complete(_launchVelocity);

    private Vector3 SamplePosition(float t) =>
      _launchVelocity * t + Physics2D.gravity * (0.5f * t * t);
  }
}