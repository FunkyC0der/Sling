using Sling.Level.Player;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class CannonProjectile : MonoBehaviour
  {
    [SerializeField] private Rigidbody2D _rigidbody;

    private float _collisionIgnoreUntilTime;
    private float _destroyDelay;
    private bool _isDestroying;

    public void Launch(
      Vector2 direction,
      float speed,
      float lifetime,
      float collisionIgnoreDuration,
      float destroyDelay)
    {
      transform.right = direction;

      _rigidbody.bodyType = RigidbodyType2D.Kinematic;
      _rigidbody.linearVelocity = direction.normalized * speed;
      _collisionIgnoreUntilTime = Time.time + collisionIgnoreDuration;
      _destroyDelay = destroyDelay;

      Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      if (IsPlayer(collision.rigidbody))
        return;

      TryScheduleDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (IsPlayer(other.attachedRigidbody) || other.TryGetComponent(out PlayerView _))
        return;

      TryScheduleDestroy();
    }

    private void TryScheduleDestroy()
    {
      if (_isDestroying || IsCollisionIgnored)
        return;

      _isDestroying = true;
      Destroy(gameObject, _destroyDelay);
    }

    private static bool IsPlayer(Rigidbody2D rigidbody) =>
      rigidbody != null && rigidbody.TryGetComponent(out PlayerView _);

    private bool IsCollisionIgnored =>
      Time.time < _collisionIgnoreUntilTime;

    private void Reset() =>
      _rigidbody = GetComponent<Rigidbody2D>();
  }
}
