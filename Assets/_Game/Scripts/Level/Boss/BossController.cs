using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using PrimeTween;
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
          weakPoint.Hide(showVFX: false);

      var cts = new CancellationTokenSource();
      AddDisposable(new DisposableToken(cts.Cancel));
      RunPhasesAsync(cts.Token).Forget();
    }

    private async UniTaskVoid RunPhasesAsync(CancellationToken cancellationToken)
    {
      for (int phaseIndex = 0; phaseIndex < _bossView.PhaseCount; phaseIndex++)
      {
        if(_model.CurrentPhaseIndex != phaseIndex)
        {
          if (_model.CurrentPhaseIndex >= 0)
          {
            _bossView.StopPhase(_model.CurrentPhaseIndex);
            await _bossView.TransitionToPhaseAsync(phaseIndex, cancellationToken);
          }
          
          _bossView.StartPhase(phaseIndex);
          _model.CurrentPhaseIndex = phaseIndex;
        }
        
        await ExecuteAndWaitResultAsync<BossPhaseController>(cancellationToken);
      }
      
      _bossView.StopPhase(_model.CurrentPhaseIndex);
      _events.OnLevelCompleted?.Invoke();
    }
  }
}
