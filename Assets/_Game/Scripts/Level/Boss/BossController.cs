using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Boss
{
  public class BossController : ControllerBase
  {
    private readonly BossView _bossView;
    private readonly BossModel _model;
    private readonly LevelEvents _events;

    public BossController(IControllerFactory factory, BossView bossView, BossModel model, LevelEvents events)
      : base(factory)
    {
      _bossView = bossView;
      _model = model;
      _events = events;
    }

    protected override void OnStart()
    {
      for (int i = 0; i < _bossView.PhaseCount; i++)
        foreach (WeakPointView weakPoint in _bossView.GetPhaseWeakPoints(i))
          weakPoint.Hide();

      var cts = new CancellationTokenSource();
      AddDisposable(new DisposableToken(cts.Cancel));
      RunPhasesAsync(cts.Token).Forget();
    }

    private async UniTaskVoid RunPhasesAsync(CancellationToken cancellationToken)
    {
      for (int phase = 0; phase < _bossView.PhaseCount; phase++)
      {
        _model.CurrentPhaseIndex = phase;
        _bossView.ActivatePhase(phase);
        await ExecuteAndWaitResultAsync<BossPhaseController>(cancellationToken);
      }
      _events.OnLevelCompleted?.Invoke();
    }
  }
}
