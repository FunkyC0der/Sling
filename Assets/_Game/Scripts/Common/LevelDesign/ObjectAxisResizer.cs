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
    [SerializeField] private BoxCollider2D _collider;
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
        
        _renderer.size = _isVertical
          ? new Vector2(_renderer.size.x, rendererAxisSize)
          : new Vector2(rendererAxisSize, _renderer.size.y);
      }
      
      if(_collider)
        _collider.size = _isVertical 
          ? new Vector2(_collider.size.x, axisSize) 
          : new Vector2(axisSize, _collider.size.y);    
    }
#endif
  }
}
