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
      
      _sequence = BlinkSequence.Create(_blinkMaterial, _blinkAmountPropertyId, blinkCount, duration, blinkAmount);

      _sequence.OnComplete(() => _spriteRenderer.sharedMaterial = originalMaterial);

      await _sequence;
    }

    public void StopBlink() => 
      _sequence.Stop();
  }
}
