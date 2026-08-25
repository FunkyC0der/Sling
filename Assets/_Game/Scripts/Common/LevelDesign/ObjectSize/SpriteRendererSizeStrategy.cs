using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  [Serializable]
  public class SpriteRendererSizeStrategy : ObjectSizeStrategy
  {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Vector2 _resizePivot;

    public override void Apply(Vector2 size)
    {
      if (_renderer == null)
        return;

      Transform target = _renderer.transform;
      Vector2 resizePivot = new Vector2(
        Mathf.Clamp(_resizePivot.x, -1f, 1f),
        Mathf.Clamp(_resizePivot.y, -1f, 1f));
      Vector2 pivotOffset = -Vector2.Scale(resizePivot, size) * 0.5f;
      Vector3 currentPosition = target.localPosition;
      bool sizeChanged = false;
      bool positionChanged = false;

      if (!Mathf.Approximately(_renderer.size.x, size.x) ||
          !Mathf.Approximately(_renderer.size.y, size.y))
      {
        _renderer.size = size;
        sizeChanged = true;
      }

      if (!Mathf.Approximately(currentPosition.x, pivotOffset.x) ||
          !Mathf.Approximately(currentPosition.y, pivotOffset.y))
      {
        target.localPosition = new Vector3(pivotOffset.x, pivotOffset.y, currentPosition.z);
        positionChanged = true;
      }

      if (!sizeChanged && !positionChanged)
        return;

#if UNITY_EDITOR
      if (sizeChanged && PrefabUtility.IsPartOfPrefabInstance(_renderer))
      {
        PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
        EditorUtility.SetDirty(_renderer);
      }

      if (positionChanged && PrefabUtility.IsPartOfPrefabInstance(target))
      {
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        EditorUtility.SetDirty(target);
      }
#endif
    }
  }
}
