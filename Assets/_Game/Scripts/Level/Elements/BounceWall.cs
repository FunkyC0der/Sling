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

      Vector2 incomingVelocity = collision.relativeVelocity;
      Vector2 normal = collision.GetContact(0).normal;
      Vector2 reflectVelocity = Vector2.Reflect(incomingVelocity, normal);
      rb.linearVelocity = reflectVelocity * _config.BounceMultiplier;
    }
  }
}
