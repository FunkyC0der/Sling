using Playtika.Controllers;
using Sling.Audio;
using Sling.Levels;

namespace Sling.Level.Session
{
  public class LevelTrackController : ControllerBase
  {
    private readonly GameModel _gameModel;
    private readonly GameConfig _gameConfig;
    private readonly AudioEvents _audioEvents;

    public LevelTrackController(IControllerFactory controllerFactory,
      GameModel gameModel,
      GameConfig gameConfig,
      AudioEvents audioEvents) 
      : base(controllerFactory)
    {
      _gameModel = gameModel;
      _gameConfig = gameConfig;
      _audioEvents = audioEvents;
    }

    protected override void OnStart()
    {
      LevelConfig levelConfig = _gameConfig.Levels[_gameModel.LevelIndex];
      _audioEvents.PlayMusic?.Invoke(levelConfig.Track);
    }
  }
}