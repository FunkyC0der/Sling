using System;
using Sling.Common.UI;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowView
  {
    public event Action OnNextLevelClicked;
    public event Action OnRestartClicked;
    public event Action OnMenuClicked;

    public LevelCompleteWindowView(VisualElement contentRoot)
    {
      contentRoot.Q<Button>(WindowNames.NextLevelButton).clicked += () => OnNextLevelClicked?.Invoke();
      contentRoot.Q<Button>(WindowNames.RestartButton).clicked   += () => OnRestartClicked?.Invoke();
      contentRoot.Q<Button>(WindowNames.MenuButton).clicked      += () => OnMenuClicked?.Invoke();
    }
  }
}
