using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class SpriteScaleSizeStrategy : ObjectSizeStrategy
  {
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _originalSize = Vector2.one;
    [SerializeField] private Vector2 _resizePivot;

    public override void Apply(Vector2 size)
    {
      if (_target == null)
        return;

      if (_originalSize.x <= 0f || _originalSize.y <= 0f)
        return;

      float scaleX = size.x / _originalSize.x;
      float scaleY = size.y / _originalSize.y;
      Vector3 currentScale = _target.localScale;
      Vector2 pivotOffset = GetPivotOffset(_resizePivot, size);
      Vector3 currentPosition = _target.localPosition;
      bool hasChanges = false;

      if (!Mathf.Approximately(currentScale.x, scaleX) || !Mathf.Approximately(currentScale.y, scaleY))
      {
        _target.localScale = new Vector3(scaleX, scaleY, currentScale.z);
        hasChanges = true;
      }

      if (!Mathf.Approximately(currentPosition.x, pivotOffset.x) ||
          !Mathf.Approximately(currentPosition.y, pivotOffset.y))
      {
        _target.localPosition = new Vector3(pivotOffset.x, pivotOffset.y, currentPosition.z);
        hasChanges = true;
      }

      if (!hasChanges)
        return;

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
