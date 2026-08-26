using System;
using UnityEngine;

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class DirectSizeSource : ObjectSizeSource
  {
    [SerializeField] private Vector2 _size = Vector2.one;

    public override Vector2 GetSize()
    {
      return _size;
    }
  }
}
