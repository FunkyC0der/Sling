using System;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.LevelComplete
{
  [Serializable]
  public class NewBestScorePulseConfig
  {
    [Min(0)] public int Count = 2;
    [Tooltip("Delay after the window open animation has finished.")]
    [Min(0f)] public float Delay = 0.15f;
    [Min(1f)] public float Scale = 1.1f;
    [Tooltip("Duration of a single pulse (grow and shrink back).")]
    [Min(0.01f)] public float Duration = 0.3f;
    public Ease Ease = Ease.OutQuad;
  }
}
