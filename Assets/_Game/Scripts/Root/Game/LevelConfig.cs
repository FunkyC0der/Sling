using System;
using Sling.Common.Scenes;

namespace Sling.Root.Game
{
  [Serializable]
  public class LevelConfig
  {
    public LevelType Type;
    public SceneReference Scene;
  }
}