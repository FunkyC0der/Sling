using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Boot
{
  public class BootstrapController : ControllerWithResultBase
  {
    public BootstrapController(IControllerFactory factory) : base(factory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await ExecuteAndWaitResultAsync<InitFirstSceneController>(ct);
      Complete();
    }
  }
}