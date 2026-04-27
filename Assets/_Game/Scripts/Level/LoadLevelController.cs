using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine.SceneManagement;

namespace Sling.Level
{
    public class LoadLevelController : ControllerWithResultBase<int>
    {
        public LoadLevelController(IControllerFactory factory) 
            : base(factory)
        {
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            var sceneName = $"Level_{Args:00}";
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}