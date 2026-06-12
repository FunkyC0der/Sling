using Sling.Audio;
using UnityEngine;

namespace Sling.Level.Elements.Bounce
{
  [RequireComponent(typeof(Collider2D))]
  public class BounceZone : MonoBehaviour
  {
    [SerializeField] private BounceZoneConfig _config;
    [SerializeField] private AudioClipEmitter _bounceClipEmitter;

    private Vector3 Forward => transform.right;
    private Vector3 Normal => transform.up;

    private void OnCollisionEnter2D(Collision2D collision)
    {
      Rigidbody2D rb = collision.rigidbody;
      if (rb == null)
        return;

      bool invert = _config.BothDirection && !IsVectorTowardsTo(rb.linearVelocity, Forward);

      Vector3 otherPositionInLocalSpace = rb.transform.position - transform.position;
      bool mirror = !IsVectorTowardsTo(otherPositionInLocalSpace, Normal);
      
      _bounceClipEmitter.PlayOneShot();
      rb.linearVelocity = CreateBounceVelocity(invert, mirror);
    }

    private Vector2 CreateBounceVelocity(bool invert, bool mirror)
    {
      Vector2 bounceDir = invert ? -Forward : Forward;
      Vector2 rotatedDir = Quaternion.Euler(0, 0, mirror ? -_config.Angle : _config.Angle) * bounceDir;
      return rotatedDir.normalized * _config.Impulse;
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

      if (_config.BothDirection)
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
