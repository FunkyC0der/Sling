using System;
using UnityEngine;

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public abstract class ObjectSizeStrategy
  {
    public abstract void Apply(Vector2 size);

    protected static Vector2 GetPivotOffset(Vector2 resizePivot, Vector2 size)
    {
      return -Vector2.Scale(resizePivot, size) * 0.5f;
    }
  }
}
