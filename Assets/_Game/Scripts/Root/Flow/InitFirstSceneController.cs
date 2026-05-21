using Playtika.Controllers;
using Sling.Root.Game;
using UnityEditor;

namespace Sling.Root.Flow
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

          int levelIndex = _gameConfig.LevelScenes.FindIndex(level => level.SceneName == editorScene);
          if (levelIndex > -1)
            _gameModel.LevelIndex = levelIndex;
        }
      }
#endif

      Complete();
    }
  }
}
