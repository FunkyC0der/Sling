using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.LevelDesign.ObjectSize
{
  public class ObjectSizeSetter : MonoBehaviour
  {
    [SerializeReference] private ObjectSizeSource _sizeSource;
    [SerializeReference] private List<ObjectSizeStrategy> _strategies = new();

    public void Apply()
    {
      if (_strategies == null)
        return;

      Vector2 size = ResolveSize();

      for (int i = 0; i < _strategies.Count; i++)
      {
        ObjectSizeStrategy strategy = _strategies[i];
        if (strategy == null)
          continue;

        strategy.Apply(size);
      }
    }

    private Vector2 ResolveSize()
    {
      return _sizeSource != null ? _sizeSource.GetSize() : Vector2.one;
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
