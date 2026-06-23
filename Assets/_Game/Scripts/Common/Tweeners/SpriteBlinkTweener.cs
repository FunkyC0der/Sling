using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  public class SpriteBlinkTweener : MonoBehaviour
  {
    private static readonly int _sDefaultBlinkAmountId = Shader.PropertyToID("_BlinkAmount");

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _blinkMaterial;
    [SerializeField] private string _blinkAmountProperty = "_BlinkAmount";

    private Sequence _sequence;
    private int _blinkAmountPropertyId = _sDefaultBlinkAmountId;

    private void Awake() => 
      _blinkAmountPropertyId = Shader.PropertyToID(_blinkAmountProperty);

    private void OnDestroy() =>
      StopBlink();

    public async UniTask PlayBlink(int blinkCount, float duration, float blinkAmount)
    {
      if (_sequence.isAlive) 
        _sequence.Stop();

      Material originalMaterial = _spriteRenderer.sharedMaterial;
      _spriteRenderer.sharedMaterial = _blinkMaterial;
      
      float halfBlinkDuration = duration / blinkCount * 0.5f;
      
      _sequence = Sequence.Create();
      for (int i = 0; i < blinkCount; i++)
      {
        _sequence.Chain(Tween.MaterialProperty(_spriteRenderer.sharedMaterial,
          _blinkAmountPropertyId,
          startValue: 0f,
          endValue: blinkAmount,
          halfBlinkDuration));
        
        _sequence.Chain(Tween.MaterialProperty(_spriteRenderer.sharedMaterial,
          _blinkAmountPropertyId,
          endValue: 0f,
          halfBlinkDuration));
      }

      _sequence.OnComplete(() => _spriteRenderer.sharedMaterial = originalMaterial);

      await _sequence.ToUniTask(cancellationToken: destroyCancellationToken);
    }

    public void StopBlink() => 
      _sequence.Stop();
  }
}
