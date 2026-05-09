using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Tweeners
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

    private void Awake() =>
      _rigidbody = GetComponent<Rigidbody2D>();

    private void Start()
    {
      if (Points.Count == 0)
        return;
      
      Vector3 initialLocalPosition = transform.localPosition;
      Transform parent = transform.parent;

      var sequence = Sequence.Create(Cycles, updateType: UpdateType.FixedUpdate);
      Vector2 currentLocalOffset = Vector2.zero;

      foreach (Vector3 point in Points)
      {
        Vector2 nextLocalOffset = point;
        float duration = Vector2.Distance(currentLocalOffset, nextLocalOffset) / Speed;
        Vector2 segmentStart = currentLocalOffset;

        sequence.Chain(Tween.Custom(segmentStart, nextLocalOffset, duration, localOffset =>
        {
          Quaternion localRotation = Quaternion.Euler(0f, 0f, transform.localEulerAngles.z);
          Vector3 newLocalPos = initialLocalPosition + localRotation * (Vector3)localOffset;
          Vector2 worldPos = parent != null
            ? parent.TransformPoint(newLocalPos)
            : newLocalPos;
          _rigidbody.MovePosition(worldPos);
        }, Ease));

        sequence.ChainDelay(DelayBeforeNextPoint);
        currentLocalOffset = nextLocalOffset;
      }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      if (Points == null || Points.Count == 0)
        return;

      Gizmos.color = Color.yellow;
      Transform parent = transform.parent;
      Vector3 localOrigin = transform.localPosition;
      Quaternion localRotation = Quaternion.Euler(0f, 0f, transform.localEulerAngles.z);

      for (int i = 0; i < Points.Count; i++)
      {
        Vector3 localPoint = localOrigin + localRotation * Points[i];
        Vector3 worldPoint = parent != null ? parent.TransformPoint(localPoint) : localPoint;
        Gizmos.DrawSphere(worldPoint, 0.15f);

        Vector3 prevLocal = i == 0 ? localOrigin : localOrigin + localRotation * Points[i - 1];
        Vector3 prevWorld = parent != null ? parent.TransformPoint(prevLocal) : prevLocal;
        Gizmos.DrawLine(prevWorld, worldPoint);
      }
    }
#endif
  }
}
