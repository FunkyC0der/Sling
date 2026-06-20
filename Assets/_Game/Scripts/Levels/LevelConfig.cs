using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using Sling.Audio;
using Sling.Common.Scenes;
using UnityEngine;

namespace Sling.Levels
{
#if UNITY_EDITOR
  [InlineProperty]
#endif
  [Serializable]
  public class LevelConfig
  {
    public Texture2D Preview;
    public LevelType Type;
    public SceneReference Scene;
    public AudioClipId Track = AudioClipId.LevelTrack1;
  }
}
