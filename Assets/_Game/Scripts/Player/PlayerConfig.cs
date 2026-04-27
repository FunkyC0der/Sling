using UnityEngine;

namespace Sling.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "AngryMeatBoy/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxDragDistance { get; private set; } = 5f;
        [field: SerializeField] public float LaunchForceMultiplier { get; private set; } = 10f;
    }
}
