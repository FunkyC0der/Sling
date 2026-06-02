using UnityEditor;
using UnityEngine;

namespace Sling.Common.LevelDesign
{
  public class HorizontalSizeEditorView : MonoBehaviour
  {
    [Min(1)]
    [SerializeField] private int _stepsCount = 1;
    [SerializeField] private int _stepSize = 1;

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

      float width = _stepsCount * _stepSize;
      
      if (_renderer)
        _renderer.size = new Vector2(width, _renderer.size.y);
      
      if(_collider)
        _collider.size = new Vector2(width, _collider.size.y);    
    }
#endif
  }
}
