using System;
using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class DelaySwitcherStrategy : SwitcherStateChangeStrategy
  {
    [Min(0f)] public float Duration = 0.5f;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      if (immediate || Duration <= 0f)
        yield break;

      yield return new WaitForSeconds(Duration);
    }
  }
}
