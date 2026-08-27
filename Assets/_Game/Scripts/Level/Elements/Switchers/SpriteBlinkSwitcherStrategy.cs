using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Sling.Common.Tweeners;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class SpriteBlinkSwitcherStrategy : SwitcherStateChangeStrategy
  {
    [Required] public SpriteBlinkTweener BlinkTweener;
    [Min(1)] public int BlinkCount = 3;
    [Min(0f)] public float BlinkDuration = 0.5f;
    [Range(0f, 1f)] public float BlinkAmount = 0.5f;

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      if (immediate)
      {
        Stop();
        yield break;
      }

      yield return BlinkTweener
        .PlayBlink(BlinkCount, BlinkDuration, BlinkAmount)
        .ToCoroutine();
    }

    public override void Stop() =>
      BlinkTweener?.StopBlink();
  }
}
