using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sling.Audio
{
  [Serializable]
  [InlineProperty]
  public class AudioClipConfig
  {
    public AudioClip AudioClip;
    public float Volume = 1;
  }
}