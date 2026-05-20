using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.UI;

namespace Sling.Boot
{
  public class MainMenuStateController : ControllerWithResultBase
  {
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;
    
    public MainMenuStateController(IControllerFactory controllerFactory, GameConfig gameConfig, GameModel gameModel) 
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
      _gameModel = gameModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      await _gameConfig.MainMenuScene.LoadSceneAsync()
        .ToUniTask(cancellationToken: ct);

      string levelScene = await ExecuteAndWaitResultAsync<SelectLevelWindowController, string>(ct);
      if(!string.IsNullOrEmpty(levelScene))
      {
        _gameModel.GameState = GameState.PlayLevels;
        _gameModel.SceneToLoad = levelScene;
      }
      
      Complete();
    }
  }
}