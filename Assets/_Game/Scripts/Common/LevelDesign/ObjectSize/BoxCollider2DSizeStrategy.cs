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

    public override void Apply(Vector2 size)
    {
      if (_collider == null)
        return;

      if (Mathf.Approximately(_collider.size.x, size.x) && Mathf.Approximately(_collider.size.y, size.y))
        return;

      _collider.size = size;

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
