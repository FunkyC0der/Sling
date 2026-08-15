using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class BoxCollider2DSizeStrategy : ObjectSizeStrategy
  {
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private Vector2 _resizePivot;

    public override void Apply(Vector2 size)
    {
      if (_collider == null)
        return;

      Vector2 resizePivot = new Vector2(
        Mathf.Clamp(_resizePivot.x, -1f, 1f),
        Mathf.Clamp(_resizePivot.y, -1f, 1f));
      Vector2 pivotOffset = -Vector2.Scale(resizePivot, size) * 0.5f;
      bool hasChanges = false;

      if (!Mathf.Approximately(_collider.size.x, size.x) ||
          !Mathf.Approximately(_collider.size.y, size.y))
      {
        _collider.size = size;
        hasChanges = true;
      }

      if (!Mathf.Approximately(_collider.offset.x, pivotOffset.x) ||
          !Mathf.Approximately(_collider.offset.y, pivotOffset.y))
      {
        _collider.offset = pivotOffset;
        hasChanges = true;
      }

      if (!hasChanges)
        return;

#if UNITY_EDITOR
      if (PrefabUtility.IsPartOfPrefabInstance(_collider))
      {
        PrefabUtility.RecordPrefabInstancePropertyModifications(_collider);
        EditorUtility.SetDirty(_collider);
      }
#endif
    }
  }
}
