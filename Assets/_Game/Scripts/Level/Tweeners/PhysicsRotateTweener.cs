using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Tweeners
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

    private void Awake() =>
      _rigidbody = GetComponent<Rigidbody2D>();

    private void Start()
    {
      if (Angles.Count == 0)
        return;

      Transform parent = transform.parent;
      float initialParentAngle = parent != null ? parent.eulerAngles.z : 0f;

      var sequence = Sequence.Create(Cycles, updateType: UpdateType.FixedUpdate);
      float currentLocalAngle = _rigidbody.rotation - initialParentAngle;

      foreach (float angle in Angles)
      {
        float targetLocalAngle = angle - initialParentAngle;
        float duration = Mathf.Abs(targetLocalAngle - currentLocalAngle) / Speed;
        float segmentStart = currentLocalAngle;

        sequence.Chain(Tween.Custom(segmentStart, targetLocalAngle, duration, localAngle =>
        {
          float parentZ = parent != null ? parent.eulerAngles.z : 0f;
          _rigidbody.MoveRotation(parentZ + localAngle);
        }, Ease));
        sequence.ChainDelay(DelayBeforeNextAngle);

        currentLocalAngle = targetLocalAngle;
      }
    }
  }
}
