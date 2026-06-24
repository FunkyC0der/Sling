using NaughtyAttributes;
using UnityEngine;

namespace Sling.Level.Player
{
  [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/PlayerConfig")]
  public class PlayerConfig : ScriptableObject
  {
    [field: SerializeField] public float MaxDragDistance { get; private set; } = 5f;
    [field: SerializeField] public float LaunchForceMultiplier { get; private set; } = 10f;

    public LayerMask GroundSurfaceLayerMask;
    
    [Min(0)] 
    public int MaxAirLaunches;
    
    [field: Header("Trajectory")]
    [field: SerializeField] public float TrajectoryHintDuration { get; private set; } = 1f;

    [field: Header("Death anim")]
    [field: SerializeField] public int DieFlickerCount { get; private set; } = 3;
    [field: SerializeField] public float DieDuration { get; private set; } = 1.5f;
    public float ReviveDuration = 1f;
    
    [Min(0)]
    public float DieCameraShakeStrength = 1f;
    
    [Min(0)]
    public float DieCameraShakeFrequency = 5f;
    
    public ParticleSystem DeathVFXPrefab;

    [Header("Landing dust")]
    public ParticleSystem LandingDustVFXPrefab;

    [field: Header("Finish")]
    [field: SerializeField] public float FinishStopDuration { get; private set; } = 0.5f;

    [Header("Physics Tweaks")] 
    public float GlobalGravity = -9.8f;

  }
}
