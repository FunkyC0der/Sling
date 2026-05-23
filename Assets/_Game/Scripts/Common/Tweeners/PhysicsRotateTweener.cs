using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  public class PhysicsRotateTweener : PhysicsTweenerBase
  {
    public List<float> Angles;
    public float Speed = 90f;
    public float DelayBeforeNextAngle = 0.5f;
    public Ease Ease = Ease.Linear;

    [Tooltip("The number of repetitions. Setting cycles to '-1' will repeat the animation indefinitely.")]
    public int Cycles = -1;

    public override void StartTween()
    {
      if (Angles.Count == 0)
        return;

      Transform parent = transform.parent;
      float initialParentAngle = parent != null ? parent.eulerAngles.z : 0f;
      float ParentDelta() => (parent != null ? parent.eulerAngles.z : 0f) - initialParentAngle;

      _sequence = Sequence.Create(Cycles, updateType: UpdateType.FixedUpdate);
      float currentAngle = _rigidbody.rotation;

      foreach (float angle in Angles)
      {
        float duration = Mathf.Abs(angle - currentAngle) / Speed;
        _sequence.Chain(Tween.Custom(currentAngle, angle, duration,
          a => _rigidbody.MoveRotation(a + ParentDelta()), Ease));
        if (DelayBeforeNextAngle > 0)
          _sequence.ChainDelay(DelayBeforeNextAngle);
        currentAngle = angle;
      }
    }
  }
}
