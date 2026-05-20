using System.Collections.Generic;
using Sling.Level.LevelComplete;
using Sling.UI;
using Sling.Utils;
using UnityEngine;

namespace Sling
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