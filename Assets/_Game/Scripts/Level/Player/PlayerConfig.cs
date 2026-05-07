using UnityEngine;

namespace Sling.Level.Player
{
  [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/PlayerConfig")]
  public class PlayerConfig : ScriptableObject
  {
    [field: SerializeField] public float MaxDragDistance { get; private set; } = 5f;
    [field: SerializeField] public float LaunchForceMultiplier { get; private set; } = 10f;
    
    [field: Header("Death anim")]
    [field: SerializeField] public int DieFlickerCount { get; private set; } = 3;
    [field: SerializeField] public float DieDuration { get; private set; } = 1.5f;
  }
}