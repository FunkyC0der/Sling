using Playtika.Controllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Boot
{
  public class InitFirstSceneController : ControllerWithResultBase
  {
#if UNITY_EDITOR
    public const string kEditorActiveSceneSessionKey = "Sling.EditorStartLevelScene";
#endif

    private readonly GameModel _gameModel;

    public InitFirstSceneController(IControllerFactory controllerFactory, GameModel gameModel)
      : base(controllerFactory)
    {
      _gameModel = gameModel;
    }

    protected override void OnStart()
    {
#if UNITY_EDITOR
      string editorScene = SessionState.GetString(kEditorActiveSceneSessionKey, "");
      
      if (!string.IsNullOrEmpty(editorScene)) 
        _gameModel.FirstLevelScene = editorScene;
#endif
      
      Complete();
    }
  }
}