using Sling.Common.Extensions;
using UnityEngine;

namespace Sling.Level.Elements.Bounce
{
  [RequireComponent(typeof(Collider2D))]
  public class BounceZone : MonoBehaviour
  {
    [SerializeField] private BounceZoneConfig _config;

    private Vector3 ReferenceDirection => transform.right;

    private void OnCollisionEnter2D(Collision2D collision)
    {
      Rigidbody2D rb = collision.rigidbody;
      if (rb == null)
        return;

      bool isRight = _config.OneDirection || IsVectorTowardsTo(rb.linearVelocity, ReferenceDirection);
      rb.linearVelocity = CreateBounceVelocity(isRight);
    }

    private Vector2 CreateBounceVelocity(bool isRight)
    {
      Vector2 bounceDir = isRight ? ReferenceDirection : -ReferenceDirection;
      Vector2 rotatedDir = Quaternion.Euler(0, 0, _config.Angle) * bounceDir;
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
      DrawBounceVelocity(true);

      if (!_config.OneDirection)
        DrawBounceVelocity(false);
    }

    private void DrawBounceVelocity(bool isRight)
    {
      const int kBounceVectorLength = 20;
      
      Vector2 origin = transform.position;
      Vector2 velocity = CreateBounceVelocity(isRight);

      Gizmos.DrawLine(origin, origin + velocity.normalized * kBounceVectorLength);
      Gizmos.DrawLine(origin, origin + velocity.normalized.Mirror(ReferenceDirection) * kBounceVectorLength);
    }
#endif
  }
}
