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
      if (_model.IsFirstRun)
      {
        _model.CurrentPhaseIndex = 0;
        
        for (int i = 0; i < _bossView.PhaseCount; i++)
          foreach (WeakPointView weakPoint in _bossView.GetPhaseWeakPoints(i))
            weakPoint.Hide();
      }
      else if (_model.CurrentPhaseIndex > 0)
      {
        int prevPhase = _model.CurrentPhaseIndex;
        _model.CurrentPhaseIndex = 0;
        
        await _bossView.TransitionToPhaseAsync(prevPhase, _model.CurrentPhaseIndex, cancellationToken);
      }

      while (true)
      {
        await ExecuteAndWaitResultAsync<BossPhaseFlowController>(cancellationToken);

        int prevPhaseIndex = _model.CurrentPhaseIndex;
        ++_model.CurrentPhaseIndex;
        
        if (_model.CurrentPhaseIndex < _bossView.PhaseCount)
        {
          await _bossView.TransitionToPhaseAsync(prevPhaseIndex, _model.CurrentPhaseIndex, cancellationToken);
          continue;
        }
        
        break;
      }
      
      _events.OnLevelCompleted?.Invoke();
    }
  }
}
