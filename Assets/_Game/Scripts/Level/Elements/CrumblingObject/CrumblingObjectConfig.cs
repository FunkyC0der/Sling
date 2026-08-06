using UnityEngine;

namespace Sling.Level.Elements.CrumblingObject
{
  [CreateAssetMenu(fileName = "CrumblingObject", menuName = "Game/Level/CrumblingObject")]
  public class CrumblingObjectConfig : ScriptableObject
  {
    [Min(0)]
    public float CrumbleAnimDelay = 1f;
    
    [Min(0)]
    public float CrumbleAnimDuration = 1f;

    [Min(0)]
    public float RespawnAnimDelay = 1f;
    
    [Min(0)]
    public float RespawnAnimDuration = 1f;

    [Range(0, 1)]
    public float DisableColliderAnimTimePoint01 = 1;

    public float FullVisibleFadeAmount = -0.1f;
    public float FullHideFadeAmount = 1f;

    public bool OnlyTopSurface = true;

    public float DisableColliderAnimTimePoint =>
      CrumbleAnimDelay + CrumbleAnimDuration * DisableColliderAnimTimePoint01;
  }
}
