using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  public class Cannon : MonoBehaviour
  {
    [InlineEditor]
    [SerializeField] private CannonConfig _config;
    
    [NaughtyAttributes.Required]
    [SerializeField] private CannonProjectile _projectilePrefab;
    
    [SerializeField] private Transform _muzzle;

    private Vector2 Direction => _muzzle.right;

    private void Awake() =>
      FireLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();

    private async UniTaskVoid FireLoopAsync(CancellationToken cancellationToken)
    {
      try
      {
        while (true)
        {
          Fire();

          await UniTask.WaitForSeconds(_config.FireInterval, cancellationToken: cancellationToken);
        }
      }
      catch (OperationCanceledException)
      {
      }
    }

    private void Fire()
    {
      CannonProjectile projectile = Instantiate(_projectilePrefab, _muzzle.position, _muzzle.rotation);
      projectile.Launch(
        Direction,
        _config.ProjectileSpeed,
        _config.ProjectileLifetime,
        _config.CollisionIgnoreDuration,
        _config.DestroyDelay);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      if (_muzzle == null)
        return;

      const int kDirectionLineLength = 20;

      Gizmos.color = Color.red;
      Gizmos.DrawLine(_muzzle.position, _muzzle.position + (Vector3)Direction.normalized * kDirectionLineLength);
    }
#endif
  }
}
