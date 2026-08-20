using Sling.Common.Extensions;
using Sling.Level.Player;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class CannonProjectile : MonoBehaviour
  {
    [SerializeField] private Rigidbody2D _rigidbody;
    
    public ParticleSystem LeftMoveDestroyVFXPrefab;
    public ParticleSystem RightMoveDestroyVFXPrefab;
    public ParticleSystem DownMoveDestroyVFXPrefab;
    public ParticleSystem UpMoveDestroyVFXPrefab;

    private float _collisionIgnoreUntilTime;
    private float _destroyDelay;
    private bool _isDestroying;
    private Vector2 _moveDirection;

    public void Launch(
      Vector2 direction,
      float speed,
      float lifetime,
      float collisionIgnoreDuration,
      float destroyDelay)
    {
      _moveDirection = direction;
      
      _rigidbody.bodyType = RigidbodyType2D.Kinematic;
      _rigidbody.linearVelocity = direction.normalized * speed;
      _collisionIgnoreUntilTime = Time.time + collisionIgnoreDuration;
      _destroyDelay = destroyDelay;

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
      Destroy(gameObject, _destroyDelay);
    }

    private void OnDestroy() => 
      SpawnDestroyVFX();

    private void SpawnDestroyVFX()
    {
      if (!gameObject.scene.isLoaded)
        return;

      ParticleSystem prefab = LeftMoveDestroyVFXPrefab;
      
      if(_moveDirection.x > 0)
        prefab = RightMoveDestroyVFXPrefab;
      else if (_moveDirection.y < 0)
        prefab = DownMoveDestroyVFXPrefab;
      else if (_moveDirection.y > 0)
        prefab = UpMoveDestroyVFXPrefab;
      
      Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private static bool IsPlayer(Rigidbody2D rigidbody) =>
      rigidbody != null && rigidbody.TryGetComponent(out PlayerView _);

    private bool IsCollisionIgnored =>
      Time.time < _collisionIgnoreUntilTime;

    private void Reset() =>
      _rigidbody = GetComponent<Rigidbody2D>();
  }
}
