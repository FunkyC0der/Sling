using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sling.Level.Boss
{
  [Serializable]
  public class AnimatorBossHitReaction : BossHitReaction
  {
    private static readonly int _hitTriggerId = Animator.StringToHash("Hit");
    private static readonly int _idleTriggerId = Animator.StringToHash("Idle");

    public Animator _animator;
    public float _duration = 0.5f;

    public override async UniTask Play(CancellationToken cancellationToken)
    {
      if (_animator == null || _animator.runtimeAnimatorController == null)
        return;

      _animator.SetTrigger(_hitTriggerId);

      await UniTask.WaitForSeconds(_duration, cancellationToken: cancellationToken);

      if (_animator != null && _animator.runtimeAnimatorController != null)
        _animator.SetTrigger(_idleTriggerId);
    }
  }
}
