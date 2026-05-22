using System;
using Sling.Common.UI;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Level.Hud
{
  public class HudView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private UIDocument _uiDocument;

    public event Action OnPauseClicked;

    private Button _pauseButton;

    private void Awake()
    {
      _pauseButton = _uiDocument.rootVisualElement.Q<Button>(WindowNames.PauseButton);
      _pauseButton.clicked += HandlePauseClicked;
    }

    private void OnDestroy()
    {
      if (_pauseButton != null)
        _pauseButton.clicked -= HandlePauseClicked;
    }

    private void HandlePauseClicked() =>
      OnPauseClicked?.Invoke();
  }
}
