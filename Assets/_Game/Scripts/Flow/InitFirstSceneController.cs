using Playtika.Controllers;
using Sling;
using Sling.Common.Extensions;
using UnityEditor;

namespace Sling.Flow
{
  public class InitFirstSceneController : ControllerWithResultBase
  {
#if UNITY_EDITOR
    public const string kEditorActiveSceneSessionKey = "Sling.EditorStartLevelScene";
#endif

    private readonly GameModel _gameModel;
    private readonly GameConfig _gameConfig;

    public InitFirstSceneController(IControllerFactory controllerFactory, GameModel gameModel, GameConfig gameConfig)
      : base(controllerFactory)
    {
      _gameModel = gameModel;
      _gameConfig = gameConfig;
    }

    protected override void OnStart()
    {
#if UNITY_EDITOR
      string editorScene = SessionState.GetString(kEditorActiveSceneSessionKey, "");

      if (!string.IsNullOrEmpty(editorScene))
      {
        _gameModel.SceneToLoad = editorScene;

        if (editorScene.StartsWith("Level"))
        {
          _gameModel.GameState = GameState.PlayLevels;

          if (_gameConfig.TryFindLevel(editorScene, out int worldIndex, out int levelIndex))
          {
            _gameModel.WorldIndex = worldIndex;
            _gameModel.LevelIndex = levelIndex;
          }
        }
      }
#endif

      Complete();
    }
  }
}
