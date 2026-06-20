using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Common.Controllers
{
  public abstract class FixedUpdateControllerBase : ControllerBase
  {
    protected FixedUpdateControllerBase(IControllerFactory controllerFactory) 
      : base(controllerFactory)
    {
    }
    
    protected override void OnStart() => 
      FixedUpdateAsync().Forget();

    private async UniTask FixedUpdateAsync()
    {
      while (!CancellationToken.IsCancellationRequested)
      {
        await UniTask.WaitForFixedUpdate(CancellationToken);
        FixedUpdate();
      }
    }

    protected abstract void FixedUpdate();
  }
}