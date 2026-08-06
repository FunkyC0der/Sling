using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [CreateAssetMenu(fileName = "Cannon", menuName = "Game/Level/Cannon")]
  public class CannonConfig : ScriptableObject
  {
    [Min(0)]
    public float FireInterval = 2f;

    [Min(0)]
    public float ProjectileSpeed = 8f;

    [Min(0)]
    public float ProjectileLifetime = 10f;

    [Min(0)]
    public float CollisionIgnoreDuration = 0.1f;

    [Min(0)]
    public float DestroyDelay = 0.05f;
  }
}
