using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;

namespace Sling.Level.Boss
{
  public class BossPhaseFlowController : ControllerWithResultBase
  {
    private readonly BossView _view;
    private readonly BossModel _model;
    private readonly AudioEvents _audioEvents;

    public BossPhaseFlowController(IControllerFactory factory, BossView view, BossModel model, AudioEvents audioEvents)
      : base(factory)
    {
      _view = view;
      _model = model;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      BossPhaseSettings phase = _view.GetPhases()[_model.CurrentPhaseIndex];
      if(!phase.Tweener.IsActive)
        phase.Start();
      
      await phase.ShowWeakPointsAnim(cancellationToken);
      
      List<WeakPointView> activeWeakPoints = new(phase.WeakPoints);
      
      while (activeWeakPoints.Count > 0)
      {
        int hitIndex = await WaitAnyHitAsync(activeWeakPoints, cancellationToken);
        WeakPointView hitWeakPoint = activeWeakPoints[hitIndex];

        activeWeakPoints.RemoveAt(hitIndex);
        
        _audioEvents.PlaySFX?.Invoke(AudioClipId.BossDamage);
        
        hitWeakPoint.HideAfterHit();
        await _view.PlayHitAnim().AttachExternalCancellation(cancellationToken);
      }
      
      phase.Stop();
      
      Complete();
    }

    private static async UniTask<int> WaitAnyHitAsync(
      IReadOnlyList<WeakPointView> weakPoints,
      CancellationToken cancellationToken)
    {
      var source = new UniTaskCompletionSource<int>();
      List<Action> handlers = new();

      try
      {
        for (int i = 0; i < weakPoints.Count; i++)
        {
          int hitIndex = i;
          Action handler = () => source.TrySetResult(hitIndex);
          handlers.Add(handler);
          weakPoints[i].OnHit += handler;
        }

        return await source.Task.AttachExternalCancellation(cancellationToken);
      }
      finally
      {
        for (int i = 0; i < handlers.Count; i++)
          weakPoints[i].OnHit -= handlers[i];
      }
    }
  }
}
