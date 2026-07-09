using System;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Infrastructure
{
  public class UpdateController : ControllerBase
  {
    private readonly UpdateEvents _updateEvents;

    public UpdateController(IControllerFactory controllerFactory, UpdateEvents updateEvents) 
      : base(controllerFactory)
    {
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      UpdateAsync(PlayerLoopTiming.Update, () => _updateEvents.OnUpdate?.Invoke()).Forget();
      UpdateAsync(PlayerLoopTiming.PostLateUpdate, () => _updateEvents.OnPostLateUpdate?.Invoke()).Forget();
      UpdateAsync(PlayerLoopTiming.FixedUpdate, () => _updateEvents.OnFixedUpdate?.Invoke()).Forget();
    }
    
    private async UniTask UpdateAsync(PlayerLoopTiming timing, Action onUpdate)
    {
      while (!CancellationToken.IsCancellationRequested)
      {
        await UniTask.Yield(timing, CancellationToken);
        onUpdate?.Invoke();
      }
    }
  }
}