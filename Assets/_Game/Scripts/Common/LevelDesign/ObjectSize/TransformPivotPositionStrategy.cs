using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class TransformPivotPositionStrategy : ObjectSizeStrategy
  {
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _resizePivot;

    public override void Apply(Vector2 size)
    {
      if (_target == null)
        return;

      Vector2 pivotOffset = -Vector2.Scale(_resizePivot, size) * 0.5f;
      Vector3 currentPosition = _target.localPosition;

      if (Mathf.Approximately(currentPosition.x, pivotOffset.x) &&
          Mathf.Approximately(currentPosition.y, pivotOffset.y))
        return;

      _target.localPosition = new Vector3(pivotOffset.x, pivotOffset.y, currentPosition.z);

#if UNITY_EDITOR
      if (PrefabUtility.IsPartOfPrefabInstance(_target))
      {
        PrefabUtility.RecordPrefabInstancePropertyModifications(_target);
        EditorUtility.SetDirty(_target);
      }
#endif
    }
  }
}
