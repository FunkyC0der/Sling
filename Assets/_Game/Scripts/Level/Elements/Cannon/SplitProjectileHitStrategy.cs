using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [Serializable]
  public class SplitProjectileHitStrategy : ProjectileHitStrategy
  {
    public CannonProjectile FragmentPrefab;

    [InlineEditor]
    public ProjectileSplitConfig Config;

    private readonly List<Vector2> _directions = new();

    public override void Apply(ProjectileHitContext context)
    {
      if (FragmentPrefab == null || Config == null)
        return;

      FillFragmentDirections(context, _directions);

      foreach (Vector2 direction in _directions)
      {
        Vector2 position = context.Position + context.MoveDirection * Config.SpawnOffset * -1;

        CannonProjectile fragment = UnityEngine.Object.Instantiate(
          FragmentPrefab, position, Quaternion.identity);

        fragment.Launch(
          direction,
          Config.FragmentSpeed,
          Config.FragmentLifetime,
          Config.CollisionIgnoreDuration,
          Config.DestroyDelay);
      }
    }

#if UNITY_EDITOR
    public override void DrawGizmos(ProjectileHitContext context)
    {
      if (Config == null)
        return;

      const float _kMaxGizmoLength = 50f;

      FillFragmentDirections(context, _directions);

      float length = Mathf.Min(Config.FragmentSpeed * Config.FragmentLifetime, _kMaxGizmoLength);

      foreach (Vector2 direction in _directions)
      {
        Vector2 start = context.Position + direction * Config.SpawnOffset;
        Gizmos.DrawLine(start, start + direction * length);
      }
    }
#endif

    private void FillFragmentDirections(ProjectileHitContext context, List<Vector2> directions)
    {
      directions.Clear();

      Vector2 baseDirection = (-context.MoveDirection).normalized;
      int fragmentCount = Mathf.Max(Config.FragmentCount, 1);

      if (fragmentCount == 1)
      {
        directions.Add(baseDirection);
        return;
      }

      for (int i = 0; i < fragmentCount; i++)
      {
        float angle = -Config.SpreadAngle / 2f + i * Config.SpreadAngle / (fragmentCount - 1);
        directions.Add(Quaternion.Euler(0, 0, angle) * baseDirection);
      }
    }
  }
}
