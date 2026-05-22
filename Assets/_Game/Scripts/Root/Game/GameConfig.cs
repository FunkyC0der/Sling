using System.Collections.Generic;
using Sling.Common.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Root.Game
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    [Header("Scenes")]
    public SceneReference MainMenuScene;
    public List<SceneReference> LevelScenes = new();

    [Header("UI")]
    public VisualTreeAsset SelectLevelWindowUxml;
    public VisualTreeAsset SelectLevelLevelItemUxml;
    public VisualTreeAsset LevelCompleteWindowUxml;
    public VisualTreeAsset PauseWindowUxml;
  }
}
