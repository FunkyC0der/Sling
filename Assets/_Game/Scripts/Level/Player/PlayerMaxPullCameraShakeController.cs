using Playtika.Controllers;
using PrimeTween;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerMaxPullCameraShakeController : ControllerBase
  {
    private const float _kKShakeCycleDuration = 0.5f;
    private const float _kKStrengthRestartEpsilon = 0.01f;

    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly UpdateEvents _updateEvents;

    private bool _forcePreview;
    private bool _isShaking;
    private float _appliedStrength;
    private float _appliedFrequency;
    private Tween _positionShake;
    private Tween _rotationShake;

    public PlayerMaxPullCameraShakeController(
      IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerConfig config,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _model = model;
      _config = config;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnUpdate += OnUpdate;
      this.AddDisposableAction(() => _updateEvents.OnUpdate -= OnUpdate);
      this.AddDisposableAction(StopShake);
    }

    protected override void OnStop() =>
      StopShake();

    private void OnUpdate()
    {
      float strength = GetDesiredStrength();
      float frequency = _config.MaxPullCameraShakeFrequency;

      if (strength <= 0f)
      {
        if (_isShaking)
          StopShake();
        return;
      }

      if (_isShaking &&
          Mathf.Abs(strength - _appliedStrength) < _kKStrengthRestartEpsilon &&
          Mathf.Approximately(frequency, _appliedFrequency))
        return;

      StartShake(strength, frequency);
    }

    private float GetDesiredStrength()
    {
      float maxStrength = _config.MaxPullCameraShakeStrength;
      if (maxStrength <= 0f)
        return 0f;

      if (_forcePreview)
        return maxStrength;

      if (!TryGetForceFraction(out float forceFraction))
        return 0f;

      float threshold = _config.MaxPullCameraShakeThreshold;
      if (threshold >= 1f)
        return forceFraction >= 1f ? maxStrength : 0f;

      float ramp01 = Mathf.InverseLerp(threshold, 1f, forceFraction);
      return maxStrength * ramp01;
    }

    private bool TryGetForceFraction(out float forceFraction)
    {
      forceFraction = 0f;

      if (!_model.IsInPreLaunch.Value)
        return false;

      float maxForce = _config.GetMaxLaunchForce();
      if (maxForce <= 0f)
        return false;

      forceFraction = _model.PreLaunchForce / maxForce;
      return forceFraction >= _config.MaxPullCameraShakeThreshold;
    }

    private void StartShake(float strength, float frequency)
    {
      StopShake();

      Camera camera = Camera.main;
      if (!camera)
        return;

      Transform transform = camera.transform;
      float orthoPosStrength = strength * camera.orthographicSize * 0.03f;
      _positionShake = Tween.ShakeLocalPosition(
        transform,
        new ShakeSettings(
          new Vector3(orthoPosStrength, orthoPosStrength),
          _kKShakeCycleDuration,
          frequency,
          enableFalloff: false,
          cycles: -1));
      _rotationShake = Tween.ShakeLocalRotation(
        transform,
        new ShakeSettings(
          new Vector3(0f, 0f, strength * 0.6f),
          _kKShakeCycleDuration,
          frequency,
          enableFalloff: false,
          cycles: -1));

      _appliedStrength = strength;
      _appliedFrequency = frequency;
      _isShaking = true;
    }

    private void StopShake()
    {
      if (_positionShake.isAlive)
        _positionShake.Stop();
      if (_rotationShake.isAlive)
        _rotationShake.Stop();

      _positionShake = default;
      _rotationShake = default;
      _isShaking = false;
    }

    [DebugMethod("Toggle Max Pull Camera Shake Preview")]
    private void ToggleForceMaxPullCameraShakePreview()
    {
      _forcePreview = !_forcePreview;
      if (!_forcePreview && GetDesiredStrength() <= 0f)
        StopShake();
    }
  }
}
