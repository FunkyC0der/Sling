using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class AnimatorTriggerSwitcherStrategy : SwitcherStateChangeStrategy
  {
    [Required] public Animator Animator;

    [AnimatorParam(nameof(Animator), AnimatorControllerParameterType.Trigger)]
    public int OnTriggerId;

    [AnimatorParam(nameof(Animator), AnimatorControllerParameterType.Trigger)]
    public int OffTriggerId;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      int triggerId = isOn ? OnTriggerId : OffTriggerId;
      int oppositeTriggerId = isOn ? OffTriggerId : OnTriggerId;

      Animator.ResetTrigger(oppositeTriggerId);
      Animator.SetTrigger(triggerId);
      yield break;
    }
  }
}
