using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;

namespace Sling.Level.Boss
{
  public class BossPhaseController : ControllerWithResultBase
  {
    private readonly BossView _view;
    private readonly BossModel _model;
    private readonly AudioEvents _audioEvents;

    public BossPhaseController(IControllerFactory factory, BossView view, BossModel model, AudioEvents audioEvents)
      : base(factory)
    {
      _view = view;
      _model = model;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      var done = new UniTaskCompletionSource();
      int remaining = _view.GetPhaseWeakPoints(_model.CurrentPhaseIndex).Count;

      foreach (WeakPointView weakPoint in _view.GetPhaseWeakPoints(_model.CurrentPhaseIndex))
      {
        weakPoint.Show();
        WeakPointView captured = weakPoint;
        Action handler = () =>
        {
          captured.Hide(showVFX: true);
          remaining--;
          if (remaining <= 0)
            done.TrySetResult();

          _view.PlayHitAnim();
          _audioEvents.PlaySFX?.Invoke(AudioClipId.BossDamage);
        };
        captured.OnHit += handler;
        AddDisposable(new DisposableToken(() => captured.OnHit -= handler));
      }

      await done.Task.AttachExternalCancellation(cancellationToken);
      
      Complete();
    }
  }
}
