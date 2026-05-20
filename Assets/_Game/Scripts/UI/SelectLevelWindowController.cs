using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.UI
{
  public class SelectLevelWindowController : ControllerWithResultBase<string>
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

      selectLevelWindowView.OnPlayClicked += OnPlayButtonClicked;

      await ct.WaitUntilCanceled();
      Complete(string.Empty);
    }

    private void OnPlayButtonClicked(int levelIndex) => 
      Complete(_gameConfig.LevelScenes[levelIndex].SceneName);
  }
}