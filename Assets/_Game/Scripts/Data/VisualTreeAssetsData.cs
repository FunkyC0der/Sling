using System;
using UnityEngine.UIElements;

namespace Sling.Data
{
  [Serializable]
  public class VisualTreeAssetsData : EntityComponentDefinition
  {
    public VisualTreeAsset SelectLevelWindow;
    public VisualTreeAsset SelectLevelLevelItem;
    public VisualTreeAsset LevelCompleteWindow;
    public VisualTreeAsset PauseWindow;
  }
}