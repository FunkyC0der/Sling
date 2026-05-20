using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  [RequireComponent(typeof(UIDocument))]
  public class LevelCompleteWindowView : MonoBehaviour
  {
    private static class Names
    {
      public const string kNextLevelButton = "NextLevelButton";
      public const string kRestartButton = "RestartButton";
      public const string kMenuButton = "MenuButton";
    }

    public event Action OnNextLevelClicked;
    public event Action OnRestartClicked;
    public event Action OnMenuClicked;

    private UIDocument _document;

    private void Awake()
    {
      _document = GetComponent<UIDocument>();

      _document.rootVisualElement.Q<Button>(Names.kNextLevelButton).clicked += () => OnNextLevelClicked?.Invoke();
      _document.rootVisualElement.Q<Button>(Names.kRestartButton).clicked += () => OnRestartClicked?.Invoke();
      _document.rootVisualElement.Q<Button>(Names.kMenuButton).clicked += () => OnMenuClicked?.Invoke();
    }
  }
}
