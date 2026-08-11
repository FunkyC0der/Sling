using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Sling.Levels
{
  [InlineProperty]
  [Serializable]
  public class WorldConfig
  {
    public string Name;
    public List<LevelConfig> Levels = new();
  }
}
