using System.Collections.Generic;
using Sling.UI;
using Sling.Utils;
using UnityEngine;

namespace Sling
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    public SceneReference MainMenuScene;
    public List<SceneReference> LevelScenes = new();

    public SelectLevelWindowView SelectLevelWindowViewPrefab;
  }
}