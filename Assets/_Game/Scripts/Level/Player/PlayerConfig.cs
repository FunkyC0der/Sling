using NaughtyAttributes;
using UnityEngine;

namespace Sling.Level.Player
{
  [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/PlayerConfig")]
  public class PlayerConfig : ScriptableObject
  {
    [field: Header("Input")]
    [field: SerializeField] public float PointerDeltaSensitivity { get; private set; } = 2f;

    [field: SerializeField] public float MaxDragDistance { get; private set; } = 5f;
    [field: SerializeField] public float LaunchForceMultiplier { get; private set; } = 10f;

    [field: Header("Launch")]
    [field: SerializeField, Range(0f, 1f), Tooltip("AimForce / MaxLaunchForce above which the strong launch clip plays")]
    public float StrongLaunchAnimThreshold { get; private set; } = 0.6f;

    [field: SerializeField, Min(0f), Tooltip("Launching state ignores ground/wall facts for this long after a launch")]
    public float LaunchGroundIgnoreTime { get; private set; } = 0.1f;

    [field: Header("Spawn")]
    [field: SerializeField, Min(0f), Tooltip("Max time Spawning waits for the first ground contact before going Airborne")]
    public float SpawnSettleTime { get; private set; } = 0.25f;

    [field: Header("Facing")]
    [field: SerializeField, Min(0f)] public float FacingVelocityThreshold { get; private set; } = 0.2f;

    [Space]
    public LayerMask GroundSurfaceLayerMask;
    
    [Min(0)] 
    public int MaxAirLaunches;
    
    [field: Header("Trajectory")]
    [field: SerializeField] public float TrajectoryHintDuration { get; private set; } = 1f;

    [field: Header("Death anim")]
    [field: SerializeField] public int DieFlickerCount { get; private set; } = 3;
    [field: SerializeField] public float DieDuration { get; private set; } = 1.5f;
    
    [Min(0)]
    public float DieCameraShakeStrength = 1f;
    
    [Min(0)]
    public float DieCameraShakeFrequency = 5f;
    
    public ParticleSystem DeathVFXPrefab;
    
    public float ReviveDuration = 1f;
    public int ReviveBlinkCount = 2;
    public float ReviveBlinkStrength = 0.5f;

    [Header("Landing dust")]
    public ParticleSystem LandingDustVFXPrefab;

    [field: Header("Finish")]
    [field: SerializeField] public float FinishStopDuration { get; private set; } = 0.5f;

    [Header("Physics Tweaks")] 
    public float GlobalGravity = -9.8f;

    public float GetMaxLaunchForce() =>
      MaxDragDistance * LaunchForceMultiplier;
  }
}
