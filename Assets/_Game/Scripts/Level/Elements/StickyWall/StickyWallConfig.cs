using UnityEngine;

namespace Sling.Level.Elements.StickyWall
{
  [CreateAssetMenu(fileName = "StickyWall", menuName = "Game/Level/StickyWall")]
  public class StickyWallConfig : ScriptableObject
  {
    [field: Min(0)]
    [field: SerializeField] public float MaxSpeed { get; private set; } = 1f;

    [field: Min(0)]
    [field: SerializeField] public float LaunchImmunityDuration { get; private set; } = 0.5f;

    public PhysicsMaterial2D PhysMaterialToApply;
  }
}
