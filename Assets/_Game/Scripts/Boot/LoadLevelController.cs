using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine.SceneManagement;

namespace Sling.Boot
{
  public class LoadLevelController : ControllerWithResultBase<string, EmptyControllerResult>
  {
    public LoadLevelController(IControllerFactory factory)
      : base(factory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await SceneManager.LoadSceneAsync(Args)
        .ToUniTask(cancellationToken: ct);
      
      Complete(new EmptyControllerResult());
    }
  }
}