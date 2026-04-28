using UnityEngine;

namespace Sling.Level.StickyWall
{
  [CreateAssetMenu(fileName = "StickyWall", menuName = "Game/Level/StickyWall")]
  public class StickyWallConfig : ScriptableObject
  {
    [Range(0, 1)] [field: SerializeField] public float Stickiness { get; private set; } = 0.5f;

    [Min(0)] [field: SerializeField] public float MaxFallSpeed { get; private set; } = 1f;
  }
}