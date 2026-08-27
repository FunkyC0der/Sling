using System;
using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class GameObjectSwitcherStrategy : SwitcherStateChangeStrategy
  {
    public GameObject Target;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      Target.SetActive(isOn);
      yield break;
    }
  }
}
