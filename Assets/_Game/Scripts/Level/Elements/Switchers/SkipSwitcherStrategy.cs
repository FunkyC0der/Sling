using System;
using System.Collections;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class SkipSwitcherStrategy : SwitcherStateChangeStrategy
  {
    public bool SkipOn;
    public bool SkipOff;
    [SerializeReference] public SwitcherStateChangeStrategy Strategy;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      if (ShouldSkip(isOn) || Strategy == null)
        yield break;

      yield return Strategy.SetState(isOn, immediate);
    }

    public override void Stop() =>
      Strategy?.Stop();

    private bool ShouldSkip(bool isOn) =>
      isOn ? SkipOn : SkipOff;
  }
}
