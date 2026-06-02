using System;
using Sling.Common.Scenes;

namespace Sling.Levels
{
  [Serializable]
  public class LevelConfig
  {
    public LevelType Type;
    public SceneReference Scene;
  }
}
