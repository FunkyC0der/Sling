using System;
using UnityEngine;

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public abstract class ObjectSizeSource
  {
    public abstract Vector2 GetSize();
  }
}
