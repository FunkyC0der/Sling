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

      Vector3 otherPositionInLocalSpace = rb.transform.position - transform.position;
      bool mirror = !IsVectorTowardsTo(otherPositionInLocalSpace, Normal);
      
      _bounceClipEmitter.PlayOneShot();
      rb.linearVelocity = CreateBounceVelocity(invert, mirror);
    }

    private bool NeedInvert(Rigidbody2D rb)
    {
      if (!_bothDirection)
        return false;

      if (IsHorizontalZone)
        return rb.GetComponent<IFaceDirectionView>()?.IsFacingLeft ?? false;
      
      return !IsVectorTowardsTo(rb.linearVelocity, Normal);
    }

    private Vector2 CreateBounceVelocity(bool invert, bool mirror)
    {
      Vector2 bounceDir = Quaternion.Euler(0, 0, _config.Angle) * Forward;

      if (mirror)
        bounceDir = Vector2.Reflect(bounceDir, Normal);
      
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
      DrawBounceVelocity(invert: false, mirror: false);
      DrawBounceVelocity(invert: false, mirror: true);

      if (_bothDirection)
      {
        DrawBounceVelocity(invert: true, mirror: false);
        DrawBounceVelocity(invert: true, mirror: true);
      }
    }

    private void DrawBounceVelocity(bool invert, bool mirror)
    {
      const int kBounceVectorLength = 20;
      
      Vector2 origin = transform.position;
      Vector2 velocity = CreateBounceVelocity(invert, mirror);

      Gizmos.DrawLine(origin, origin + velocity.normalized * kBounceVectorLength);
    }
#endif
  }
}
