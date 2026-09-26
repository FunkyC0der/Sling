using System;
using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [Serializable]
  public class VFXProjectileHitStrategy : ProjectileHitStrategy
  {
    public ParticleSystem LeftMoveVFXPrefab;
    public ParticleSystem RightMoveVFXPrefab;
    public ParticleSystem DownMoveVFXPrefab;
    public ParticleSystem UpMoveVFXPrefab;

    public override void Apply(ProjectileHitContext context)
    {
      ParticleSystem prefab = SelectPrefab(context.MoveDirection);

      if (prefab == null)
        return;

      ParticleSystem vfx = UnityEngine.Object.Instantiate(prefab, context.Position, Quaternion.identity);
      Vector3 absScale = new(Mathf.Abs(context.Scale.x), Mathf.Abs(context.Scale.y), Mathf.Abs(context.Scale.z));
      vfx.transform.localScale = Vector3.Scale(vfx.transform.localScale, absScale);
    }

    private ParticleSystem SelectPrefab(Vector2 moveDirection)
    {
      if (moveDirection.x > 0)
        return RightMoveVFXPrefab;

      if (moveDirection.y < 0)
        return DownMoveVFXPrefab;

      if (moveDirection.y > 0)
        return UpMoveVFXPrefab;

      return LeftMoveVFXPrefab;
    }
  }
}
