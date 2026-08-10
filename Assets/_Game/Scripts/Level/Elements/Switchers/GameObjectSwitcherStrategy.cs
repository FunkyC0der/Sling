using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class GameObjectSwitcherStrategy : SwitcherStateChangeStrategy
  {
    [SerializeField] private GameObject _gameObject;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      _gameObject.SetActive(isOn);
      yield break;
    }
  }
}
