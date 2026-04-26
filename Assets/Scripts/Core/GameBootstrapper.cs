using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private HudView _hudView;
    [SerializeField] private LevelResultView _levelResultView;

    private void Start()
    {
        var factory = new ControllerFactory();

        factory.Register<BootstrapController>(() => new BootstrapController(factory));
        factory.Register<GameLoopController>(() => new GameLoopController(factory));
        factory.Register<MainMenuController>(() => new MainMenuController(factory, _mainMenuView));
        factory.Register<LevelSessionController>(() => new LevelSessionController(factory));
        factory.Register<LoadLevelController>(() => new LoadLevelController(factory));
        factory.Register<LevelGameplayController>(() => new LevelGameplayController(factory));
        factory.Register<PlayerController>(() => new PlayerController(factory, _playerConfig));
        factory.Register<HazardsController>(() => new HazardsController(factory));
        factory.Register<FinishController>(() => new FinishController(factory));
        factory.Register<HudController>(() => new HudController(factory, _hudView));
        factory.Register<LevelResultController>(() => new LevelResultController(factory, _levelResultView));

        var root = new GameRootController(factory);
        root.LaunchTree(this.GetCancellationTokenOnDestroy());
    }
}
