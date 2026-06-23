using Cysharp.Threading.Tasks;
using Sling.Common.Tweeners;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Finish
{
  [RequireComponent(typeof(Collider2D))]
  public class FinishZoneView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private SpriteBlinkTweener _blinkTweener;
    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private float _blinkDuration = 0.5f;
    [SerializeField] private float _blinkAmount = 0.5f;

    public UniTask Blink() => 
      _blinkTweener.PlayBlink(_blinkCount, _blinkDuration, _blinkAmount);
  }
}
