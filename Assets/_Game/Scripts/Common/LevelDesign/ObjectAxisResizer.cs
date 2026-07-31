using UnityEditor;
using UnityEngine;

namespace Sling.Common.LevelDesign
{
  public class ObjectAxisResizer : MonoBehaviour
  {
    [Min(1)]
    [SerializeField] private int _stepsCount = 1;
    [SerializeField] private float _stepSize = 1;
    [SerializeField] private bool _isVertical;
    [SerializeField] private float _rendererSizeToAdd;

    [Header("References")] 
    [SerializeField] private BoxCollider2D[] _colliders;
    [SerializeField] private SpriteRenderer _renderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
      EditorApplication.delayCall -= ApplySize;
      EditorApplication.delayCall += ApplySize;
    }

    private void ApplySize()
    {
      EditorApplication.delayCall -= ApplySize;

      float axisSize = _stepsCount * _stepSize;
      
      if (_renderer)
      {
        float rendererAxisSize = axisSize + _rendererSizeToAdd;
        float currentRendererAxisSize = _isVertical ? _renderer.size.y : _renderer.size.x;

        if (!Mathf.Approximately(currentRendererAxisSize, rendererAxisSize))
        {
          _renderer.size = _isVertical
            ? new Vector2(_renderer.size.x, rendererAxisSize)
            : new Vector2(rendererAxisSize, _renderer.size.y);

          if (PrefabUtility.IsPartOfPrefabInstance(_renderer))
          {
            PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
            EditorUtility.SetDirty(_renderer);
          }
        }
      }

      foreach (BoxCollider2D boxCollider in _colliders)
      {
        if(!boxCollider)
          continue;

        float currentColliderAxisSize = _isVertical ? boxCollider.size.y : boxCollider.size.x;

        if (!Mathf.Approximately(currentColliderAxisSize, axisSize))
        {
          boxCollider.size = _isVertical
            ? new Vector2(boxCollider.size.x, axisSize)
            : new Vector2(axisSize, boxCollider.size.y);

          if (PrefabUtility.IsPartOfPrefabInstance(boxCollider))
          {
            PrefabUtility.RecordPrefabInstancePropertyModifications(boxCollider);
            EditorUtility.SetDirty(boxCollider);
          }
        }
      }
    }
#endif
  }
}
