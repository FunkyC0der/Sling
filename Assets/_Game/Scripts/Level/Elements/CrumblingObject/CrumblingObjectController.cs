using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using PrimeTween;
using Sling.Common.Extensions;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Elements.CrumblingObject
{
  public class CrumblingObjectController : ControllerBase<CrumblingObject>
  {
    private readonly LevelEvents _levelEvents;

    private CrumblingObject _view;
    private CancellationTokenSource _cycleCts;
    private Sequence _sequence;
    private bool _isCrumbling;

    public CrumblingObjectController(
      IControllerFactory controllerFactory,
      LevelEvents levelEvents)
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
    }

    protected override void OnStart()
    {
      _view = Args;

      _view.TriggerZone.OnCollideEnter += OnCollideEnter;
      this.AddDisposableAction(() => _view.TriggerZone.OnCollideEnter -= OnCollideEnter);

      _levelEvents.OnPlayerDeathStarted += ForceRespawn;
      this.AddDisposableAction(() => _levelEvents.OnPlayerDeathStarted -= ForceRespawn);

      this.AddDisposableAction(CancelCycle);
    }

    private void OnCollideEnter(Collision2D collision)
    {
      if (_isCrumbling)
        return;

      if (collision.rigidbody == null || !collision.rigidbody.TryGetComponent(out PlayerView _))
        return;

      if (_view.Config.OnlyTopSurface && !_view.IsTopSurfaceContact(collision))
        return;

      CrumbleCycleAsync().Forget();
    }

    private void ForceRespawn()
    {
      if (!_isCrumbling)
        return;

      ForceRespawnAsync().Forget();
    }

    private async UniTaskVoid CrumbleCycleAsync()
    {
      CancellationTokenSource cts = StartCycle();

      try
      {
        CancellationToken ct = cts.Token;
        CrumblingObjectConfig config = _view.Config;

        await TweenFade(
            config.FullVisibleFadeAmount,
            config.FullHideFadeAmount,
            config.CrumbleAnimDuration,
            config.CrumbleAnimDelay)
          .InsertCallback(atTime: 0, PlayCrumbleVFX)
          .InsertCallback(atTime: config.DisableColliderAnimTimePoint, DisableColliderAndStopVFX)
          .WithCancellation(ct);

        await FadeInAndEnableColliderAsync(config.RespawnAnimDelay, ct);
      }
      finally
      {
        CompleteCycle(cts);
      }
    }

    private async UniTaskVoid ForceRespawnAsync()
    {
      CancellationTokenSource cts = StartCycle();

      try
      {
        CancellationToken ct = cts.Token;
        StopCrumbleVFX();
        await FadeInAndEnableColliderAsync(delay: 0f, ct);
      }
      finally
      {
        CompleteCycle(cts);
      }
    }

    private CancellationTokenSource StartCycle()
    {
      CancelCycle();
      _isCrumbling = true;
      _cycleCts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
      return _cycleCts;
    }

    private void CancelCycle()
    {
      if (_sequence.isAlive)
        _sequence.Stop();

      if (_cycleCts == null)
        return;

      CancellationTokenSource cts = _cycleCts;
      _cycleCts = null;
      cts.Cancel();
      cts.Dispose();
    }

    private void CompleteCycle(CancellationTokenSource cts)
    {
      if (_cycleCts != cts)
        return;

      _cycleCts.Dispose();
      _cycleCts = null;
      _isCrumbling = false;
    }

    private async UniTask FadeInAndEnableColliderAsync(float delay, CancellationToken ct)
    {
      CrumblingObjectConfig config = _view.Config;
      await TweenFade(
          _view.FadeAmount,
          config.FullVisibleFadeAmount,
          config.RespawnAnimDuration,
          delay)
        .WithCancellation(ct);

      await WaitUntilNoBlockingOverlap(ct);
      _view.Collider.enabled = true;
    }

    private Sequence TweenFade(
      float startValue,
      float endValue,
      float duration,
      float delay)
    {
      _sequence = Sequence.Create()
        .ChainDelay(delay)
        .Chain(Tween.Custom(
          _view,
          startValue,
          endValue,
          duration,
          (_, value) => _view.SetFadeAmount(value)));
      return _sequence;
    }

    private async UniTask WaitUntilNoBlockingOverlap(CancellationToken ct)
    {
      if (_view.Collider.enabled)
        return;

      await UniTask.WaitUntil(
        () => !_view.HasBlockingOverlap(),
        PlayerLoopTiming.FixedUpdate,
        ct);
    }

    private void PlayCrumbleVFX()
    {
      if (_view.CrumbleVFX == null)
        return;

      _view.CrumbleVFX.Play(true);
    }

    private void DisableColliderAndStopVFX()
    {
      _view.Collider.enabled = false;
      StopCrumbleVFX();
    }

    private void StopCrumbleVFX()
    {
      if (_view.CrumbleVFX == null)
        return;

      _view.CrumbleVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
  }
}
