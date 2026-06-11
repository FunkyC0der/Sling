using System;
using Sling.Audio;
using Sling.Common.Scenes;

namespace Sling.Levels
{
  [Serializable]
  public class LevelConfig
  {
    public LevelType Type;
    public SceneReference Scene;
    public AudioClipId Track = AudioClipId.LevelTrack1;
  }
}
