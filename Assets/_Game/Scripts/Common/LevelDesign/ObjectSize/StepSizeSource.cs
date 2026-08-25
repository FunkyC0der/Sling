using System;
using UnityEngine;

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class StepSizeSource : ObjectSizeSource
  {
    [SerializeField] private Vector2 _stepSize = Vector2.one;
    [Min(1)]
    [SerializeField] private Vector2Int _stepsCount = Vector2Int.one;

    public override Vector2 GetSize()
    {
      Vector2Int stepsCount = new Vector2Int(
        Mathf.Max(1, _stepsCount.x),
        Mathf.Max(1, _stepsCount.y));
      return Vector2.Scale(stepsCount, _stepSize);
    }
  }
}
