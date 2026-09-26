using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  public readonly struct ProjectileHitContext
  {
    public readonly Vector2 Position;
    public readonly Vector2 MoveDirection;
    public readonly Vector3 Scale;

    public ProjectileHitContext(Vector2 position, Vector2 moveDirection, Vector3 scale)
    {
      Position = position;
      MoveDirection = moveDirection;
      Scale = scale;
    }
  }
}
