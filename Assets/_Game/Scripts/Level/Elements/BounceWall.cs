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

      bool movingWithUp = Vector3.Dot(rb.linearVelocity, transform.up) > 0;
      Vector2 bounceDir = movingWithUp ? transform.up : -(Vector2)transform.up;

      Vector2 toObject = (rb.position - (Vector2)transform.position).normalized;
      Vector2 rotatedDir = RotateTowards(bounceDir, toObject, _config.Angle);

      rb.linearVelocity = rotatedDir.normalized * _config.Impulse;
    }

    private Vector2 RotateTowards(Vector2 current, Vector2 target, float maxAngle)
    {
      float angle = Vector2.SignedAngle(current, target);
      float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);
      return (Quaternion.Euler(0, 0, clampedAngle) * current);
    }
  }
}
