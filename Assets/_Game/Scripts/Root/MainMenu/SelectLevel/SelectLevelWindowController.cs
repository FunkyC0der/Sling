using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Root.Game;
using UnityEngine;

namespace Sling.Root.MainMenu.SelectLevel
{
  public class SelectLevelWindowController : ControllerWithResultBase<int>
  {
    private readonly GameConfig _gameConfig;

    public SelectLevelWindowController(IControllerFactory controllerFactory, GameConfig gameConfig)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      SelectLevelWindowView selectLevelWindowView = Object.Instantiate(_gameConfig.SelectLevelWindowViewPrefab);

      selectLevelWindowView.SetLevels(_gameConfig.LevelScenes
        .Select(item => new LevelItemViewData { Name = item.SceneName })
        .ToArray());

      selectLevelWindowView.OnPlayClicked += Complete;

      await ct.WaitUntilCanceled();
      Complete(0);
    }
  }
}
