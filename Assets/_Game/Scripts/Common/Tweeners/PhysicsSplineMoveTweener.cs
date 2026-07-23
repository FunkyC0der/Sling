using PrimeTween;
using UnityEngine;
using UnityEngine.Splines;

namespace Sling.Common.Tweeners
{
  public class PhysicsSplineMoveTweener : PhysicsTweenerBase
  {
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField, Min(0)] private int _splineIndex;
    [SerializeField, Min(0.01f)] private float _speed = 5f;
    [SerializeField, Min(0f)] private float _delayBeforeNextCycle = 0.5f;
    [SerializeField] private Ease _ease = Ease.Linear;

    [Tooltip("The number of repetitions. Setting cycles to '-1' will repeat the animation indefinitely.")]
    [SerializeField] private int _cycles = -1;

    public override void StartTween()
    {
      if (_splineContainer == null ||
          _splineIndex < 0 ||
          _splineIndex >= _splineContainer.Splines.Count ||
          _splineContainer.Splines[_splineIndex].Count < 2 ||
          _speed <= 0f)
        return;

      float duration = _splineContainer.CalculateLength(_splineIndex) / _speed;
      if (duration <= 0f)
        return;

      _sequence = Sequence.Create(_cycles, updateType: UpdateType.FixedUpdate);
      _sequence.Chain(Tween.Custom(
        0f,
        1f,
        duration,
        t =>
        {
          Vector3 worldPosition = _splineContainer.EvaluatePosition(_splineIndex, t);
          _rigidbody.MovePosition(worldPosition);
        },
        _ease));

      if (_delayBeforeNextCycle > 0f)
        _sequence.ChainDelay(_delayBeforeNextCycle);
    }
  }
}
