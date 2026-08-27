using System;
using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class SpriteColorSwitcherStrategy : SwitcherStateChangeStrategy
  {
    public SpriteRenderer SpriteRenderer;
    public Color OnColor = Color.white;
    public Color OffColor = Color.white;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      SpriteRenderer.color = isOn ? OnColor : OffColor;
      yield break;
    }
  }
}
