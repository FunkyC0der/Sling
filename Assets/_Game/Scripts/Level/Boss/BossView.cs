using System;
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
  [Serializable]
  public class BossPhaseSettings
  {
    public PhysicsTweenerBase Tweener;
    public List<WeakPointView> WeakPoints;
    
    public Vector3 InitialPosition { get; set; }
    public float InitialRotation { get; set; }
  }

  public class BossView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private Transform _bossBody;
    [SerializeField] private float _phaseTransitionDuration = 0.5f;
    [SerializeField] private List<BossPhaseSettings> _phases;
    [SerializeField] private ShakeSettings _hitShakeSettings;
    [SerializeField] private float _blinkAmount;
    [SerializeField] private int _hitBlinkCount = 3;

    private SpriteBlinkTweener[] _blinkTweeners;

    public int PhaseCount => _phases.Count;

    public IReadOnlyList<WeakPointView> GetPhaseWeakPoints(int index) => _phases[index].WeakPoints;
    public IReadOnlyList<BossPhaseSettings> GetPhases() => _phases;

    private void Awake() => 
      _blinkTweeners = GetComponentsInChildren<SpriteBlinkTweener>();

    private void Start()
    {
      foreach (BossPhaseSettings phases in _phases)
      {
        phases.InitialPosition = phases.Tweener.Rigidbody.position;
        phases.InitialRotation = phases.Tweener.Rigidbody.rotation;
      }

    }

    public void StopPhase(int phaseIndex)
    {
      BossPhaseSettings phase = _phases[phaseIndex];
      phase.Tweener.StopTween();
      
      _bossBody.SetParent(transform);
      
      phase.Tweener.Rigidbody.position = phase.InitialPosition;
      phase.Tweener.Rigidbody.rotation = phase.InitialRotation;
    }

    public async UniTask TransitionWithStartPhaseAsync(int phaseIndex, CancellationToken cancellationToken)
    {
      Vector3 origScale = _bossBody.lossyScale;
      
      Tween scaleDownTween = Tween.Scale(_bossBody, Vector3.zero, _phaseTransitionDuration / 2);
      await scaleDownTween.WithCancellation(cancellationToken);

      StartPhase(phaseIndex);

      Tween scaleUpTween = Tween.Scale(_bossBody, origScale, _phaseTransitionDuration / 2);
      await scaleUpTween.WithCancellation(cancellationToken);
    }

    public void StartPhase(int phaseIndex)
    {
      BossPhaseSettings phase = _phases[phaseIndex];
      _bossBody.SetParent(phase.Tweener.transform);
      
      _bossBody.localPosition = Vector3.zero;
      _bossBody.localRotation = Quaternion.identity;
      
      phase.Tweener.StartTween();
    }

    [Button]
    public async UniTask PlayHitAnim()
    {
      PlayShakeAnim().Forget();
      
      foreach (SpriteBlinkTweener blinkTweener in _blinkTweeners) 
        blinkTweener.PlayBlink(_hitBlinkCount, _hitShakeSettings.duration, _blinkAmount).Forget();

      await UniTask.WaitForSeconds(_hitShakeSettings.duration);
    }
    
    private async UniTask PlayShakeAnim() =>
      await Tween.ShakeLocalPosition(_bossBody, _hitShakeSettings);
  }
}
