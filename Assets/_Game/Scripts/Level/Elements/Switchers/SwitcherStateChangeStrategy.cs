using System;
using System.Collections;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public abstract class SwitcherStateChangeStrategy
  {
    public abstract IEnumerator SetState(bool isOn, bool immediate);

    public virtual void Stop()
    {
    }
  }
}
