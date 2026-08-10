using System;
using UnityEngine;

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public abstract class ObjectSizeStrategy
  {
    public abstract void Apply(Vector2 size);
  }
}
