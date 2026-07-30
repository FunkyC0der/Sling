using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using Sling.Common.Tweeners;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Boss
{
  public class BossView : MonoBehaviour, IUniqueView
  {
    private static readonly int _hitTriggerId = Animator.StringToHash("Hit");
    private static readonly int _idleTriggerId = Animator.StringToHash("Idle");

    [SerializeField] private Transform _bossBody;
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<BossPhaseSettings> _phases;
    [SerializeField] private ShakeSettings _hitShakeSettings;
    [SerializeField] private float _blinkAmount;
    [SerializeField] private int _hitBlinkCount = 3;
    [SerializeField] private float _phaseTransitionMoveSpeed = 10f;
    
    private SpriteBlinkTweener[] _blinkTweeners;

    public int PhaseCount => _phases.Count;

    private void Awake() => 
      _blinkTweeners = GetComponentsInChildren<SpriteBlinkTweener>();

    public void Init()
    {
      foreach (BossPhaseSettings phase in _phases)
      {
        phase.SaveInitialTransform();

        foreach (WeakPointView weakPoint in phase.WeakPoints) 
          weakPoint.Hide();
      }
    }

    public BossPhaseSettings GetPhase(int index) => 
      _phases[index];

    public bool IsPhaseStarted(int index) =>
      GetPhase(index).Tweener.IsActive;

    public void StartPhase(int index)
    {
      BossPhaseSettings phase = GetPhase(index);

      SetAnimatorController(phase.AnimatorController);

      if (!HasAnimator() && phase.BodySprite != null)
        _bodySprite.sprite = phase.BodySprite;
      
      phase.AttachBossBody(_bossBody);
      phase.Start();
    }

    public async UniTask TransitionToPhaseAsync(int phaseIndex, int nextPhaseIndex, CancellationToken cancellationToken)
    {
      BossPhaseSettings phase = _phases[phaseIndex];
      phase.Stop();
      
      BossPhaseSettings nextPhase = _phases[nextPhaseIndex];
      nextPhase.Tweener.Rigidbody.position = phase.Tweener.Rigidbody.position;
      
      await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

      nextPhase.AttachBossBody(_bossBody);
      phase.ResetToInitialTransform();
      
      await phase.HideWeakPointsAnim(cancellationToken);

      float distance = Vector2.Distance(nextPhase.Tweener.Rigidbody.position, nextPhase.InitialPosition);

      await Tween.RigidbodyMovePosition(
          nextPhase.Tweener.Rigidbody,
          nextPhase.InitialPosition,
          duration: distance / _phaseTransitionMoveSpeed,
          Easing.Standard(Ease.InOutSine))
        .WithCancellation(cancellationToken);
    }

    [Button("Play Hit Anim")]
    private void PlayHitAnimInEditor() =>
      PlayHitAnim(this.GetCancellationTokenOnDestroy()).Forget();

    public async UniTask PlayHitAnim(CancellationToken cancellationToken)
    {
      bool hasAnimator = HasAnimator();

      if (hasAnimator)
        _animator.SetTrigger(_hitTriggerId);

      PlayShakeAnim().Forget();
      
      foreach (SpriteBlinkTweener blinkTweener in _blinkTweeners) 
        blinkTweener.PlayBlink(_hitBlinkCount, _hitShakeSettings.duration, _blinkAmount).Forget();

      await UniTask.WaitForSeconds(_hitShakeSettings.duration, cancellationToken: cancellationToken);

      if (hasAnimator)
        _animator.SetTrigger(_idleTriggerId);
    }

    private void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
      if (_animator == null)
        return;

      _animator.runtimeAnimatorController = animatorController;

      if (HasAnimator())
      {
        _animator.Rebind();
        _animator.Update(0f);
      }
    }

    private bool HasAnimator() =>
      _animator != null && _animator.runtimeAnimatorController != null;

    private async UniTask PlayShakeAnim() =>
      await Tween.ShakeLocalPosition(_bossBody, _hitShakeSettings);
  }
}
