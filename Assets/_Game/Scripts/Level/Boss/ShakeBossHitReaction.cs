using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Boss
{
  [Serializable]
  public class ShakeBossHitReaction : BossHitReaction
  {
    public Transform _target;
    public ShakeSettings _shakeSettings;

    public override async UniTask Play(CancellationToken cancellationToken) =>
      await Tween.ShakeLocalPosition(_target, _shakeSettings).WithCancellation(cancellationToken);
  }
}
