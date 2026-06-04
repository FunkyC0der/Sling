using UnityEngine;

namespace Sling.Level.Elements.Bounce
{
  [CreateAssetMenu(fileName = "BounceWall", menuName = "Game/Level/BounceWall")]
  public class BounceZoneConfig : ScriptableObject
  {
    [Range(0, 90)]
    public float Angle = 45f;

    [Min(0)]
    public float Impulse = 2f;

    public bool BothDirection;
  }
}