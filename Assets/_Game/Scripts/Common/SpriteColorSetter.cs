using UnityEngine;

namespace Sling.Common
{
  [RequireComponent(typeof(SpriteRenderer))]
  public class SpriteColorSetter : MonoBehaviour
  {
    private SpriteRenderer _spriteRenderer;

    private void Awake() => 
      _spriteRenderer = GetComponent<SpriteRenderer>();

    public void SetColor(Color color) =>
      _spriteRenderer.color = color;
  }
}