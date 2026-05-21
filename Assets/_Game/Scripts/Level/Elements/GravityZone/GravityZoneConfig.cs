using UnityEngine;

namespace Sling.Level.Elements.GravityZone
{
  [CreateAssetMenu(fileName = "GravityZone", menuName = "Game/Level/GravityZone")]
  public class GravityZoneConfig : ScriptableObject
  {
    public float Force = 2f;
  }
}