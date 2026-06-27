using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sling.LevelLoading
{
  public class LoadLevelController : ControllerWithResultBase<string, EmptyControllerResult>
  {
    public LoadLevelController(IControllerFactory factory)
      : base(factory)
    {
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Args);
      asyncOperation.allowSceneActivation = false;

      await UniTask.WaitWhile(() => asyncOperation.progress < 0.9f, cancellationToken: cancellationToken);

      asyncOperation.allowSceneActivation = true;
      await UniTask.WaitUntil(() => asyncOperation.isDone, cancellationToken: cancellationToken);
      
      Complete(new EmptyControllerResult());
    }
  }
}