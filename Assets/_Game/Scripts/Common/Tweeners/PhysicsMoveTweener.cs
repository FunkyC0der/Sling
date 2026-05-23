using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  public class PhysicsMoveTweener : PhysicsTweenerBase
  {
    public List<Vector3> Points;
    public float Speed = 5f;
    public float DelayBeforeNextPoint = 0.5f;
    public Ease Ease = Ease.Linear;

    [Tooltip("The number of repetitions. Setting cycles to '-1' will repeat the animation indefinitely.")]
    public int Cycles = -1;

    public override void StartTween()
    {
      if (Points.Count == 0)
        return;

      Vector3 initialLocalPosition = transform.localPosition;
      
      _sequence = Sequence.Create(Cycles, updateType: UpdateType.FixedUpdate);
      Vector3 currentLocalOffset = Vector3.zero;

      foreach (Vector3 point in Points)
      {
        float duration = Vector3.Distance(currentLocalOffset, point) / Speed;
        _sequence.Chain(Tween.Custom(currentLocalOffset, point, duration,
          offset => _rigidbody.MovePosition(LocalOffsetToWorld(initialLocalPosition, offset)),
          Ease));
        if (DelayBeforeNextPoint > 0)
          _sequence.ChainDelay(DelayBeforeNextPoint);
        currentLocalOffset = point;
      }
    }

    private Vector3 LocalOffsetToWorld(Vector3 initialLocalPosition, Vector3 offset)
    {
      Vector3 localPos = initialLocalPosition + transform.localRotation * offset;
      Transform parent = transform.parent;
      return parent != null ? parent.TransformPoint(localPos) : localPos;
    }
  }
}
