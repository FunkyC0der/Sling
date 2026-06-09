using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(Animator))]
  public class PlayerAnimatorView : MonoBehaviour, IGameObjectView
  {
    [SerializeField, Required] private Animator _animator;
    [SerializeField, Required] private SpriteRenderer _spriteRenderer;

    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _jumpTriggerId;
    
    [SerializeField] 
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
    private int _landTriggerId;
    
    public void Jump() => 
      _animator.SetTrigger(_jumpTriggerId);

    public void Land() => 
      _animator.SetTrigger(_landTriggerId);

    public async UniTask Die(float duration, int flickerCount, CancellationToken cancellationToken)
    {
      Color originalColor = _spriteRenderer.color;
      float flickerPeriod = duration / flickerCount;

      for (int i = 0; i < flickerCount; ++i)
      {
        _spriteRenderer.color = Color.red;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f, cancellationToken: cancellationToken);
        _spriteRenderer.color = originalColor;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f, cancellationToken: cancellationToken);
      }
    }
  }
}
