using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine.SceneManagement;

namespace Sling.Level
{
  public class LoadLevelController : ControllerWithResultBase<string, EmptyControllerResult>
  {
    public LoadLevelController(IControllerFactory factory)
      : base(factory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      await SceneManager.LoadSceneAsync(Args, LoadSceneMode.Single)
        .ToUniTask(cancellationToken: cancellationToken);
      
      Complete(new EmptyControllerResult());
    }
  }
}