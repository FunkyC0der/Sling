using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelController : ControllerWithResultBase<int, LevelSceneContext>
{
    public LoadLevelController(IControllerFactory factory) : base(factory) { }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        var sceneName = $"Level_{Args:00}";
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single)
            .ToUniTask(cancellationToken: cancellationToken);

        var scene = SceneManager.GetSceneByName(sceneName);
        var context = FindContextInScene(scene);

        if (context == null)
        {
            Fail(new System.Exception($"LevelSceneContext not found in scene '{sceneName}'"));
            return;
        }

        context.LevelEvents = new LevelEvents();
        Complete(context);
    }

    private LevelSceneContext FindContextInScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var ctx = root.GetComponentInChildren<LevelSceneContext>();
            if (ctx != null)
                return ctx;
        }
        return null;
    }
}
