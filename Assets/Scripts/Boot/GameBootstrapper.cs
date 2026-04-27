using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerConfig _playerConfig;

    private void Start()
    {
        var factory = new ControllerFactory();

        factory.Register(() => new BootstrapController(factory));
        factory.Register(() => new GameLoopController(factory));
        factory.Register(() => new LevelSessionController(factory));
        factory.Register(() => new BuildLevelFactoryController(factory, _playerConfig));

        var root = new GameRootController(factory);
        root.LaunchTree(this.GetCancellationTokenOnDestroy());
    }
}
