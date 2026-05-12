using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Tweeners
{
  [Serializable]
  public struct BezierSegment
  {
    public Vector3 Point;
    public Vector3 Control;
  }

  [RequireComponent(typeof(Rigidbody2D))]
  public class PhysicsBezierMoveTweener : MonoBehaviour
  {
    public List<BezierSegment> Segments;
    public float Speed = 5f;
    public float DelayBeforeNextSegment = 0.5f;
    public Ease Ease = Ease.Linear;

    [Tooltip("The number of repetitions. Setting cycles to '-1' will repeat the animation indefinitely.")]
    public int Cycles = -1;

    private Rigidbody2D _rigidbody;

    private void Awake() =>
      _rigidbody = GetComponent<Rigidbody2D>();

    private void Start()
    {
      if (Segments.Count == 0)
        return;

      Vector3 initialLocalPosition = transform.localPosition;
      var sequence = Sequence.Create(Cycles, updateType: UpdateType.FixedUpdate);
      Vector3 currentLocalOffset = Vector3.zero;

      foreach (BezierSegment seg in Segments)
      {
        float duration = Vector3.Distance(currentLocalOffset, seg.Point) / Speed;
        Vector3 start = currentLocalOffset;
        
        sequence.Chain(Tween.Custom(
          0f, 
          1f, 
          duration,
          t => 
            _rigidbody.MovePosition(
              LocalOffsetToWorld(initialLocalPosition, SampleQuadraticBezier(start, seg.Control, seg.Point, t))),
          Ease));
        
        sequence.ChainDelay(DelayBeforeNextSegment);
        currentLocalOffset = seg.Point;
      }
    }

    private static Vector3 SampleQuadraticBezier(Vector3 p0, Vector3 control, Vector3 p1, float t)
    {
      float mt = 1f - t;
      return mt * mt * p0 + 2f * mt * t * control + t * t * p1;
    }

    private Vector3 LocalOffsetToWorld(Vector3 initialLocalPosition, Vector3 offset)
    {
      Vector3 localPos = initialLocalPosition + transform.localRotation * offset;
      Transform parent = transform.parent;
      return parent != null ? parent.TransformPoint(localPos) : localPos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      if (Segments == null || Segments.Count == 0)
        return;

      Vector3 initialLocalPosition = transform.localPosition;
      Vector3 currentLocalOffset = Vector3.zero;

      foreach (BezierSegment seg in Segments)
      {
        Vector3 worldEnd = LocalOffsetToWorld(initialLocalPosition, seg.Point);
        Vector3 worldControl = LocalOffsetToWorld(initialLocalPosition, seg.Control);
        Vector3 worldStart = LocalOffsetToWorld(initialLocalPosition, currentLocalOffset);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(worldEnd, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldControl, 0.08f);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(worldStart, worldControl);
        Gizmos.DrawLine(worldControl, worldEnd);

        Gizmos.color = Color.yellow;
        const int steps = 16;
        Vector3 prev = worldStart;
        for (int i = 1; i <= steps; i++)
        {
          float t = i / (float)steps;
          Vector3 next = LocalOffsetToWorld(initialLocalPosition, SampleQuadraticBezier(currentLocalOffset, seg.Control, seg.Point, t));
          Gizmos.DrawLine(prev, next);
          prev = next;
        }

        currentLocalOffset = seg.Point;
      }
    }
#endif
  }
}
