using UnityEngine;

namespace Sling.Level.Elements.StickyWall
{
  [CreateAssetMenu(fileName = "StickyWall", menuName = "Game/Level/StickyWall")]
  public class StickyWallConfig : ScriptableObject
  {
    [field: Min(0)]
    [field: SerializeField] public float MaxFallSpeed { get; private set; } = 1f;
  }
}