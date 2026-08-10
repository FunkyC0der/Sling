using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  public class ObjectSizeSetter : MonoBehaviour
  {
    [SerializeField] private Vector2 _size = Vector2.one;
    [SerializeReference] private List<ObjectSizeStrategy> _strategies = new();

    public Vector2 Size
    {
      get => _size;
      set
      {
        _size = value;
        Apply();
      }
    }

    public float SizeX
    {
      get => _size.x;
      set
      {
        _size.x = value;
        Apply();
      }
    }

    public float SizeY
    {
      get => _size.y;
      set
      {
        _size.y = value;
        Apply();
      }
    }

    public void SetSize(float x, float y)
    {
      _size = new Vector2(x, y);
      Apply();
    }

    public void Apply()
    {
      if (_strategies == null)
        return;

      for (int i = 0; i < _strategies.Count; i++)
      {
        ObjectSizeStrategy strategy = _strategies[i];
        if (strategy == null)
          continue;

        strategy.Apply(_size);
      }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
      EditorApplication.delayCall -= ApplyDelayed;
      EditorApplication.delayCall += ApplyDelayed;
    }

    private void ApplyDelayed()
    {
      EditorApplication.delayCall -= ApplyDelayed;

      if (this == null)
        return;

      Apply();
    }
#endif
  }
}
