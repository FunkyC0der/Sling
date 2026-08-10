using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public abstract class SwitcherStateChangeStrategy : MonoBehaviour
  {
    public abstract IEnumerator SetState(bool isOn, bool immediate);
  }
}
