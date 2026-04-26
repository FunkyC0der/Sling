using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;
using UnityEngine.SceneManagement;

public class LoadLevelController : ControllerWithResultBase<int, IControllerFactory>
{
    private readonly PlayerConfig _playerConfig;
    private readonly HudView _hudView;
    private readonly LevelResultView _levelResultView;

    public LoadLevelController(
        IControllerFactory factory,
        PlayerConfig playerConfig,
        HudView hudView,
        LevelResultView levelResultView) : base(factory)
    {
        _playerConfig = playerConfig;
        _hudView = hudView;
        _levelResultView = levelResultView;
    }

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

        var events = new LevelEvents();
        var levelFactory = BuildLevelFactory(context, events);
        Complete(levelFactory);
    }

    private IControllerFactory BuildLevelFactory(LevelSceneContext context, LevelEvents events)
    {
        var f = new ControllerFactory();
        f.Register<LevelGameplayController>(() => new LevelGameplayController(f, events));
        f.Register<PlayerController>(() => new PlayerController(f, context.PlayerView, _playerConfig));
        f.Register<HazardsController>(() => new HazardsController(f, context.Hazards, events));
        f.Register<FinishController>(() => new FinishController(f, context.FinishView, events));
        f.Register<HudController>(() => new HudController(f, _hudView, events));
        f.Register<LevelResultController>(() => new LevelResultController(f, _levelResultView));
        return f;
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
