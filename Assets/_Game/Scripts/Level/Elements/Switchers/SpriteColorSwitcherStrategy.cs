using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class SpriteColorSwitcherStrategy : SwitcherStateChangeStrategy
  {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _onColor = Color.white;
    [SerializeField] private Color _offColor = Color.white;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      _spriteRenderer.color = isOn ? _onColor : _offColor;
      yield break;
    }
  }
}
