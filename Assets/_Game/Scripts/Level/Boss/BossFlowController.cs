using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Boss
{
  public class BossFlowController : ControllerWithResultBase
  {
    private readonly BossView _bossView;
    private readonly BossModel _model;
    private readonly LevelEvents _events;

    public BossFlowController(IControllerFactory factory, BossView bossView, BossModel model, LevelEvents events)
      : base(factory)
    {
      _bossView = bossView;
      _model = model;
      _events = events;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      for (int i = 0; i < _bossView.PhaseCount; i++)
        foreach (WeakPointView weakPoint in _bossView.GetPhaseWeakPoints(i))
          weakPoint.Hide(showVFX: false);
      
      if (_model.IsFirstRun)
      {
        _model.CurrentPhaseIndex = 0;
        _bossView.StartPhase(_model.CurrentPhaseIndex);
      }
      else if (_model.CurrentPhaseIndex > 0)
      {
        _bossView.StopPhase(_model.CurrentPhaseIndex);
        _model.CurrentPhaseIndex = 0;
        await _bossView.TransitionWithStartPhaseAsync(_model.CurrentPhaseIndex, cancellationToken);
      }

      while (true)
      {
        await ExecuteAndWaitResultAsync<BossPhaseController>(cancellationToken);
        _bossView.StopPhase(_model.CurrentPhaseIndex);

        ++_model.CurrentPhaseIndex;
        if (_model.CurrentPhaseIndex >= _bossView.PhaseCount)
          break;
        
        await _bossView.TransitionWithStartPhaseAsync(_model.CurrentPhaseIndex, cancellationToken);
      }
      
      _events.OnLevelCompleted?.Invoke();
    }
  }
}
