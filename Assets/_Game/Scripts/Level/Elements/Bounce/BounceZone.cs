using NaughtyAttributes;
using Sling.Audio;
using Sling.Common.Extensions;
using Sling.Level.Common;
using UnityEngine;

namespace Sling.Level.Elements.Bounce
{
  [RequireComponent(typeof(Collider2D))]
  public class BounceZone : MonoBehaviour
  {
    [SerializeField] private BounceZoneConfig _config;
    [SerializeField] private AudioClipEmitter _bounceClipEmitter;
    [SerializeField, Required] private Animator _animator;

    [SerializeField]
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _bounceTriggerId;

    [SerializeField] private bool _invert;
    [SerializeField] private bool _bothDirection;

    private Vector3 Forward => transform.right;
    private Vector3 Normal => transform.up;
    private bool IsHorizontalZone => Mathf.Abs(Forward.x) > Mathf.Abs(Forward.y);

    private void OnCollisionEnter2D(Collision2D collision)
    {
      Rigidbody2D rb = collision.rigidbody;
      if (rb == null)
        return;

      bool invert = NeedInvert(rb);

      _bounceClipEmitter.PlayOneShot();
      _animator.SetTrigger(_bounceTriggerId);
      rb.linearVelocity = CreateBounceVelocity(invert);
    }

    private bool NeedInvert(Rigidbody2D rb)
    {
      if (!_bothDirection)
        return false;

      if (IsHorizontalZone)
        return rb.GetComponent<IFaceDirectionView>()?.IsFacingLeft ?? false;
      
      return !IsVectorTowardsTo(rb.linearVelocity, Normal);
    }

    private Vector2 CreateBounceVelocity(bool invert)
    {
      if(_invert && !_bothDirection)
        invert = !invert;
      
      Vector2 bounceDir = Quaternion.Euler(0, 0, _config.Angle) * Forward;

      if (invert)
        bounceDir = Vector2.Reflect(bounceDir, Forward);
      
      return bounceDir.normalized * _config.Impulse;
    }
    
    private static bool IsVectorTowardsTo(Vector2 vector, Vector2 target) =>
      Vector2.Dot(vector, target) > 0;
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      if (_config == null)
        return;

      Gizmos.color = Color.cyan;
      DrawBounceVelocity(invert: false);

      if (_bothDirection) 
        DrawBounceVelocity(invert: true);
    }

    private void DrawBounceVelocity(bool invert)
    {
      const int kBounceVectorLength = 20;
      
      Vector2 origin = transform.position;
      Vector2 velocity = CreateBounceVelocity(invert);

      Gizmos.DrawLine(origin, origin + velocity.normalized * kBounceVectorLength);
    }
#endif
  }
}
