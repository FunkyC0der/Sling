using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using Sling.Common.Tweeners;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerAnimatorView : MonoBehaviour, IGameObjectView
  {
    [SerializeField, Required] private PlayerConfig _config;
    [SerializeField, Required] private Animator _animator;
    [SerializeField, Required] private SpriteRenderer _spriteRenderer;
    [SerializeField, Required] private SpriteBlinkTweener _blinkTweener;

    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _inAirTriggerId;
    
    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _landTriggerId;
    
    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _wallSlideTriggerId;

    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Float)]
    private int _inAirBlendFloatId;
    
    
    public void InAir() => 
      _animator.SetTrigger(_inAirTriggerId);

    public void Land() => 
      _animator.SetTrigger(_landTriggerId);

    public void WallSlide() => 
      _animator.SetTrigger(_wallSlideTriggerId);

    public void SetInAirBlendValue01(float value) => 
      _animator.SetFloat(_inAirBlendFloatId, value);

    public async UniTask Die(CancellationToken cancellationToken)
    {
      gameObject.SetActive(false);
      Instantiate(_config.DeathVFXPrefab, transform.position, Quaternion.identity);
      
      _ = Tween.ShakeCamera(Camera.main, _config.DieCameraShakeStrength, frequency: _config.DieCameraShakeFrequency);
      
      await UniTask.WaitForSeconds(_config.DieDuration, cancellationToken: cancellationToken);
    }

    public async UniTask Revive()
    {
      gameObject.SetActive(true);
      await _blinkTweener.PlayBlink(_config.ReviveBlinkCount, _config.ReviveDuration, _config.ReviveBlinkStrength);
    }
  }
}
