using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Level.Boss
{
  public class BossPhaseController : ControllerWithResultBase
  {
    private readonly BossView _bossView;
    private readonly BossModel _model;

    public BossPhaseController(IControllerFactory factory, BossView bossView, BossModel model)
      : base(factory)
    {
      _bossView = bossView;
      _model = model;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      var done = new UniTaskCompletionSource();
      int remaining = _bossView.GetPhaseWeakPoints(_model.CurrentPhaseIndex).Count;

      foreach (WeakPointView weakPoint in _bossView.GetPhaseWeakPoints(_model.CurrentPhaseIndex))
      {
        weakPoint.Show();
        WeakPointView captured = weakPoint;
        Action handler = () =>
        {
          captured.Hide();
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
