using System.Collections.Generic;
using Sling.Audio;
using Sling.Common.Scenes;
using Sling.Infrastructure.FixedViewport;
using Sling.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    public AudioConfig AudioConfig;
    public FixedViewportConfig FixedViewport = new();
    
    public SceneReference MainMenuScene;
    public List<LevelConfig> Levels = new();

    public VisualTreeAsset SelectLevelWindowUxml;
    public VisualTreeAsset SelectLevelLevelItemUxml;
    public VisualTreeAsset LevelCompleteWindowUxml;
    public VisualTreeAsset PauseWindowUxml;
  }
}
