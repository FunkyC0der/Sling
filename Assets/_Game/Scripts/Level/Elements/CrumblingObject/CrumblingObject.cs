using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using Sling.Common.Collission;
using Sling.Level.Player;
using UnityEngine;

namespace Sling.Level.Elements.CrumblingObject
{
  public class CrumblingObject : MonoBehaviour
  {
    private static readonly int _sFadeAmountId = Shader.PropertyToID("_FadeAmount");

    [SerializeField] private CrumblingObjectConfig _config;
    [SerializeField, Required] private Renderer _renderer;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private TriggerZone _triggerZone;

    private MaterialPropertyBlock _propertyBlock;
    private bool _isCrumbling;

    private void Awake()
    {
      _propertyBlock = new MaterialPropertyBlock();
      SetFadeAmount(_config.FullVisibleFadeAmount);

      _triggerZone.OnCollideEnter += OnCollideEnter;
    }

    private void OnDestroy() =>
      _triggerZone.OnCollideEnter -= OnCollideEnter;

    private void OnCollideEnter(Collision2D collision)
    {
      if (_isCrumbling)
        return;

      if (collision.rigidbody == null || !collision.rigidbody.TryGetComponent(out PlayerView _))
        return;

      if (_config.OnlyTopSurface && !IsTopSurfaceContact(collision))
        return;

      CrumbleCycleAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private static bool IsTopSurfaceContact(Collision2D collision)
    {
      for (int i = 0; i < collision.contactCount; i++)
      {
        // Contact normal points from the other collider toward this one.
        // Hitting the top surface means the other object is above → normal ≈ down.
        if (collision.GetContact(i).normal.y < -0.5f)
          return true;
      }

      return false;
    }

    private async UniTaskVoid CrumbleCycleAsync(CancellationToken ct)
    {
      _isCrumbling = true;

      try
      {
        await TweenFade(
          _config.FullVisibleFadeAmount,
          _config.FullHideFadeAmount,
          _config.CrumbleAnimDuration,
          _config.TimeToCrumble - _config.CrumbleAnimDuration,
          ct);

        _collider.enabled = false;

        await TweenFade(
          _config.FullHideFadeAmount,
          _config.FullVisibleFadeAmount,
          _config.CrumbleAnimDuration,
          _config.Cooldown - _config.CrumbleAnimDuration,
          ct);
        
        _collider.enabled = true;
      }
      finally
      {
        _isCrumbling = false;
      }
    }

    private async UniTask TweenFade(
      float startValue,
      float endValue,
      float duration,
      float delay,
      CancellationToken ct)
    {
      await Sequence.Create()
        .ChainDelay(delay)
        .Chain(Tween.Custom(
          this,
          startValue,
          endValue,
          duration,
          (_, value) => SetFadeAmount(value)))
        .WithCancellation(ct);
    }

    private void SetFadeAmount(float value)
    {
      _renderer.GetPropertyBlock(_propertyBlock);
      _propertyBlock.SetFloat(_sFadeAmountId, value);
      _renderer.SetPropertyBlock(_propertyBlock);
    }
  }
}
