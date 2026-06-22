using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Flow;
using Sling.Infrastructure;
using Sling.Infrastructure.Analytics;

namespace Sling
{
  public class GameLoopController : ControllerBase
  {
    private readonly GameModel _gameModel;

    public GameLoopController(IControllerFactory factory, GameModel gameModel)
      : base(factory)
    {
      _gameModel = gameModel;
    }

    protected override void OnStart() => 
      GameLoopAsync().Forget();

    private async UniTask GameLoopAsync()
    {
      Execute<UpdateController>();
      Execute<AudioController>();
      Execute<AnalyticsController>();
      
      await ExecuteAndWaitResultAsync<InitFirstSceneController>(CancellationToken);
      await ExecuteAndWaitResultAsync<InitUnityServicesFlowController>(CancellationToken);

      while (!CancellationToken.IsCancellationRequested)
      {
        if(_gameModel.GameState == GameState.MainMenu)
          await ExecuteAndWaitResultAsync<MainMenuStateController>(CancellationToken);
        else if (_gameModel.GameState == GameState.PlayLevels)
          await ExecuteAndWaitResultAsync<PlayLevelsStateController>(CancellationToken);
      }
    }
  }
}
