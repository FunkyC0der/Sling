using System;

namespace Sling.Level.Elements.Cannon
{
  [Serializable]
  public abstract class ProjectileHitStrategy
  {
    public abstract void Apply(ProjectileHitContext context);

#if UNITY_EDITOR
    public virtual void DrawGizmos(ProjectileHitContext context)
    {
    }
#endif
  }
}
