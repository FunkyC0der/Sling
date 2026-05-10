using UnityEngine;

namespace Sling.Level.Elements
{
  [CreateAssetMenu(fileName = "BounceWall", menuName = "Game/Level/BounceWall")]
  public class BounceWallConfig : ScriptableObject
  {
    [Min(0)]
    public float BounceMultiplier;
  }
}