using UnityEngine;

[CreateAssetMenu(fileName = "MovingSawConfig", menuName = "AngryMeatBoy/MovingSawConfig")]
public class MovingSawConfig : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; } = 2f;
}
