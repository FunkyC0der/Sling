using UnityEngine;

namespace Sling.Level.Elements.MovingLevelPart
{
  [CreateAssetMenu(fileName = "MovingLevelPart", menuName = "Game/Level/Moving Level Part")]
  public class MovingLevelPartConfig : ScriptableObject
  {
    [Min(0)] public float MoveSpeed = 5f;
    [Min(0)] public float ResetDuration = 0.25f;
  }
}
