using System.Collections.Generic;
using Sling.Audio;
using Sling.Common.Extensions;
using Sling.Level.Player;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class CannonProjectile : MonoBehaviour
  {
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeReference] public List<ProjectileHitStrategy> HitStrategies = new();

    private float _collisionIgnoreUntilTime;
    private float _destroyDelay;
    private bool _isDestroying;
    private Vector2 _moveDirection;
    private AudioClipEmitter _breakClipEmitter;

    public void Launch(
      Vector2 direction,
      float speed,
      float lifetime,
      float collisionIgnoreDuration,
      float destroyDelay,
      AudioClipEmitter breakClipEmitter)
    {
      _moveDirection = direction;
      
      _rigidbody.bodyType = RigidbodyType2D.Kinematic;
      _rigidbody.linearVelocity = direction.normalized * speed;
      _collisionIgnoreUntilTime = Time.time + collisionIgnoreDuration;
      _destroyDelay = destroyDelay;
      _breakClipEmitter = breakClipEmitter;

      if (_moveDirection.x > 0) 
        transform.localScale = transform.localScale.Multiply(-1);

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

      if (_breakClipEmitter != null)
        _breakClipEmitter.Play();

      ApplyHitStrategies();
      Destroy(gameObject, _destroyDelay);
    }

    private void ApplyHitStrategies()
    {
      ProjectileHitContext context = new(transform.position, _moveDirection, transform.lossyScale, _breakClipEmitter);

      foreach (ProjectileHitStrategy strategy in HitStrategies)
        strategy?.Apply(context);
    }

    private static bool IsPlayer(Rigidbody2D rigidbody) =>
      rigidbody != null && rigidbody.TryGetComponent(out PlayerView _);

    private bool IsCollisionIgnored =>
      Time.time < _collisionIgnoreUntilTime;

    private void Reset() =>
      _rigidbody = GetComponent<Rigidbody2D>();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      ProjectileHitContext context = new(transform.position, transform.right, transform.lossyScale, null);

      foreach (ProjectileHitStrategy strategy in HitStrategies)
        strategy?.DrawGizmos(context);
    }
#endif
  }
}
