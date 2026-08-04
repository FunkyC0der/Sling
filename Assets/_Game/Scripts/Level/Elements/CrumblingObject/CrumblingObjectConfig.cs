using UnityEngine;

namespace Sling.Level.Elements.CrumblingObject
{
  [CreateAssetMenu(fileName = "CrumblingObject", menuName = "Game/Level/CrumblingObject")]
  public class CrumblingObjectConfig : ScriptableObject
  {
    [Min(0)]
    public float TimeToCrumble = 1f;

    [Min(0)]
    public float Cooldown = 1f;

    [Min(0)]
    public float CrumbleAnimDuration = 1f;

    public float FullVisibleFadeAmount = -0.1f;
    public float FullHideFadeAmount = 1f;

    public bool OnlyTopSurface = true;
  }
}
