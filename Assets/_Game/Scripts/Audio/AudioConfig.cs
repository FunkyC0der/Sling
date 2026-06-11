using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sling.Audio
{
  [CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/AudioConfig")]
  public class AudioConfig : SerializedScriptableObject
  {
    public Dictionary<AudioClipId, AudioClipConfig> AudioClips;
  }
}