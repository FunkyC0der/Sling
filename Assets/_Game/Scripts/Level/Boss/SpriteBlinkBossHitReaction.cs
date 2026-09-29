using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sling.Common.Tweeners;

namespace Sling.Level.Boss
{
  [Serializable]
  public class SpriteBlinkBossHitReaction : BossHitReaction
  {
    public List<SpriteBlinkTweener> _tweeners = new();
    public int _blinkCount = 2;
    public float _duration = 0.5f;
    public float _blinkAmount = 0.8f;

    public override async UniTask Play(CancellationToken cancellationToken) =>
      await UniTask.WhenAll(_tweeners.Select(t => t.PlayBlink(_blinkCount, _duration, _blinkAmount)))
        .AttachExternalCancellation(cancellationToken);

    public override void Stop()
    {
      foreach (SpriteBlinkTweener tweener in _tweeners)
        if (tweener != null)
          tweener.StopBlink();
    }
  }
}
