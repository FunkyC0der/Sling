using UnityEngine;

namespace Sling.Level.Elements
{
  [RequireComponent(typeof(Collider2D))]
  public class BounceWall : MonoBehaviour
  {
    [SerializeField] private BounceWallConfig _config;

    private void OnCollisionEnter2D(Collision2D collision)
    {
      Rigidbody2D rb = collision.rigidbody;
      if (rb == null)
        return;

      
      Vector2 bounceDir = transform.up;
      
      if(!_config.OneDirection && !IsVectorTowardsTo(rb.linearVelocity, transform.up)) 
        bounceDir *= -1;

      Vector2 toObject = (rb.position - (Vector2)transform.position).normalized;
      Vector2 rotatedDir = RotateTowards(bounceDir, toObject, _config.Angle);

      rb.linearVelocity = rotatedDir.normalized * _config.Impulse;
    }
    
    private static bool IsVectorTowardsTo(Vector2 vector, Vector2 target) =>
      Vector2.Dot(vector, target) > 0;

    private static Vector2 RotateTowards(Vector2 current, Vector2 target, float maxAngle)
    {
      float angle = Vector2.SignedAngle(current, target);
      float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);
      return (Quaternion.Euler(0, 0, clampedAngle) * current);
    }
  }
}
