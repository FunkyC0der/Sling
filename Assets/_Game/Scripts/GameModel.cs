namespace Sling
{
  public class GameModel
  {
    public string SceneToLoad;
    public int WorldIndex;
    public int LevelIndex;
    public GameState GameState = GameState.MainMenu;
  }
}