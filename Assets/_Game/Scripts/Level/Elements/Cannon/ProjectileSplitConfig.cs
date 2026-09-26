using UnityEngine;

namespace Sling.Level.Elements.Cannon
{
  [CreateAssetMenu(fileName = "ProjectileSplit", menuName = "Game/Level/ProjectileSplit")]
  public class ProjectileSplitConfig : ScriptableObject
  {
    [Min(1)]
    public int FragmentCount = 4;

    [Range(0, 360)]
    public float SpreadAngle = 180f;

    [Min(0)]
    public float FragmentSpeed = 20f;

    [Min(0)]
    public float FragmentLifetime = 5f;

    [Min(0)]
    public float CollisionIgnoreDuration = 0.1f;

    [Min(0)]
    public float DestroyDelay = 0.08f;

    public float SpawnOffset = 0.5f;
  }
}
