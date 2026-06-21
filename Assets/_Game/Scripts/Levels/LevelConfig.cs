using System;
using Sirenix.OdinInspector;
using Sling.Audio;
using Sling.Common.Scenes;
using UnityEngine;

namespace Sling.Levels
{
  [InlineProperty]
  [Serializable]
  public class LevelConfig
  {
    public Texture2D Preview;
    public LevelType Type;
    public SceneReference Scene;
    public AudioClipId Track = AudioClipId.LevelTrack1;
  }
}
