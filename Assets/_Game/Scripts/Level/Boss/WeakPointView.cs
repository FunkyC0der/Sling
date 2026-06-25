using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Boss
{
  [RequireComponent(typeof(Collider2D))]
  public class WeakPointView : MonoBehaviour
  {
    public event Action OnHit;

    [SerializeField] private ParticleSystem _hideVFXPrefab;
    [SerializeField] private TweenSettings _showScaleTweenSettings;

    private Vector3 _origScale;

    public bool IsHidden => !gameObject.activeSelf;

    private void Awake() => 
      _origScale = transform.localScale;

    private void OnCollisionEnter2D() => 
      OnHit?.Invoke();
    
    public async UniTask Show(CancellationToken cancellationToken)
    {
      if (gameObject.activeSelf)
        return;
      
      gameObject.SetActive(true);
      await Tween.Scale(transform, new TweenSettings<Vector3>(Vector3.zero, _origScale, _showScaleTweenSettings))
        .WithCancellation(cancellationToken);
    }

    public void Hide() => 
      gameObject.SetActive(false);

    public async UniTask HideWithAnim(CancellationToken cancellationToken)
    {
      await Tween.Scale(transform, new TweenSettings<Vector3>(_origScale, Vector3.zero, _showScaleTweenSettings))
        .WithCancellation(cancellationToken);
      
      Hide();
    }

    public void HideAfterHit()
    {
      Hide();
      Instantiate(_hideVFXPrefab, transform.position, Quaternion.identity);
    }
  }
}
