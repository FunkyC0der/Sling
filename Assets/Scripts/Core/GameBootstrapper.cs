using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private MovingSawConfig _movingSawConfig;
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
        factory.Register<LoadLevelController>(() => new LoadLevelController(factory, _playerConfig, _movingSawConfig, _hudView, _levelResultView));

        var root = new GameRootController(factory);
        root.LaunchTree(this.GetCancellationTokenOnDestroy());
    }
}
