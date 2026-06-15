using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Level.Boss
{
  public class BossPhaseController : ControllerWithResultBase
  {
    private readonly BossView _view;
    private readonly BossModel _model;

    public BossPhaseController(IControllerFactory factory, BossView view, BossModel model)
      : base(factory)
    {
      _view = view;
      _model = model;
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
        };
        captured.OnHit += handler;
        AddDisposable(new DisposableToken(() => captured.OnHit -= handler));
      }

      await done.Task.AttachExternalCancellation(cancellationToken);
      
      Complete();
    }
  }
}
