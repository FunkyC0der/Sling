using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PhysicsMoveTweener : MonoBehaviour
  {
    public List<Vector3> Points;
    public float Speed = 5f;
    public float DelayBeforeNextPoint = 0.5f;
    public Ease Ease = Ease.Linear;

    [Tooltip("The number of repetitions. Setting cycles to '-1' will repeat the animation indefinitely.")]
    public int Cycles = -1;

    private Rigidbody2D _rigidbody;
    private Sequence _sequence;

    private void Awake() =>
      _rigidbody = GetComponent<Rigidbody2D>();

    private void OnDestroy() =>
      _sequence.Stop();

    private void Start()
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
