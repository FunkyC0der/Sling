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

    /// <summary>How deep (world units, along X) a body sinks into the slowdown zone on a horizontal hit.</summary>
    [field: Min(0)]
    [field: SerializeField] public float StickDepth { get; private set; } = 1f;

    public PhysicsMaterial2D PhysMaterialToApply;
  }
}
