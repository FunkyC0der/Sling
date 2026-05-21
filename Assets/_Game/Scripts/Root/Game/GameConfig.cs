using System.Collections.Generic;
using Sling.Common.Scenes;
using Sling.Level.LevelComplete;
using Sling.Root.MainMenu.SelectLevel;
using UnityEngine;

namespace Sling.Root.Game
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    [Header("Scenes")]
    public SceneReference MainMenuScene;
    public List<SceneReference> LevelScenes = new();

    [Header("UI")]
    public SelectLevelWindowView SelectLevelWindowViewPrefab;
    public LevelCompleteWindowView LevelCompleteWindowViewPrefab;
  }
}
