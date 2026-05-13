using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level;
using Sling.Utils;
using VContainer.Unity;

namespace Sling.Boot
{
  public class PlayLevelsStateController : ControllerWithResultBase
  {
    private readonly GameModel _gameModel;
    
    public PlayLevelsStateController(IControllerFactory factory, GameModel gameModel) 
      : base(factory)
    {
      _gameModel = gameModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      do
      {
        await ExecuteAndWaitResultAsync<LoadLevelController, string>(_gameModel.SceneToLoad, ct);
        
        LifetimeScope levelScope = await ExecuteAndWaitResultAsync<BuildLevelScopeController, LifetimeScope>(ct);
        
        await ExecuteAndWaitResultAsync<LevelSessionController>(levelScope.GetControllerFactory(), ct);

        levelScope.Dispose();
      } while (true);
    }
  }
}