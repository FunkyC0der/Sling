using UnityEngine;

namespace Sling.Level.Elements.GravityZone
{
  [CreateAssetMenu(fileName = "GravityZone", menuName = "Game/Level/GravityZone")]
  public class GravityZoneConfig : ScriptableObject
  {
    public float Force = 2f;

    [Range(0, 1)]
    public float GravityScaleMultiplier = 0.5f;
  }
}