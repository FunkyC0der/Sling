using UnityEngine;

namespace Sling.Level.Elements.Tumbleweed
{
  [CreateAssetMenu(fileName = "Tumbleweed", menuName = "Game/Level/Tumbleweed")]
  public class TumbleweedConfig : ScriptableObject
  {
    [Header("Spawn")]
    public float SpawnIntervalMin = 4f;
    public float SpawnIntervalMax = 10f;
    public int MaxAlive = 3;
    [Tooltip("Spawned right at the level start")]
    public int InitialCount = 2;
    public Sprite[] Sprites;

    [Header("Movement")]
    public float SpeedMin = 3f;
    public float SpeedMax = 5f;
    public float HopHeightMin = 1f;
    public float HopHeightMax = 3f;
    [Tooltip("Distance covered by a single hop")]
    public float HopLength = 6f;
    [Tooltip("Minimum time from spawn until it disappears")]
    public float MinLifetime = 4f;
    [Tooltip("Maximum time from spawn until it disappears")]
    public float MaxLifetime = 10f;

    [Header("Fade")]
    public float FadeInDuration = 0.5f;
    public float FadeOutDuration = 0.5f;

    [Header("Surfaces")]
    [Tooltip("World size of one level cell")]
    public float CellSize = 4f;
    public int MinSurfaceCells = 3;
    public LayerMask GroundMask;
    public LayerMask BlockerMask;
    public float CameraPadding = 8f;

    [Header("Rendering")]
    public bool PixelSnap = true;
  }
}
