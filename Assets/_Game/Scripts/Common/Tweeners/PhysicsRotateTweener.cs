using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PhysicsRotateTweener : MonoBehaviour
  {
    public List<float> Angles;
    public float Speed = 90f;
    public float DelayBeforeNextAngle = 0.5f;
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
        _sequence.ChainDelay(DelayBeforeNextAngle);
        currentAngle = angle;
      }
    }
  }
}
