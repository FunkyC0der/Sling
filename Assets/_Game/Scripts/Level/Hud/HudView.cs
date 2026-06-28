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
    private Label _elapsedTimeLabel;
    private Label _playerDeathCountLabel;

    private void Awake()
    {
      VisualElement root = _uiDocument.rootVisualElement;
      
      _pauseButton = root.Q<Button>(WindowNames.PauseButton);
      _pauseButton.clicked += HandlePauseClicked;

      _elapsedTimeLabel = root.Q<Label>(WindowNames.kElapsedTime);
      _playerDeathCountLabel = root.Q<Label>(WindowNames.kPlayerDeathCount);
    }

    private void OnDestroy()
    {
      if (_pauseButton != null)
        _pauseButton.clicked -= HandlePauseClicked;
    }

    private void HandlePauseClicked() =>
      OnPauseClicked?.Invoke();

    public void SetLevelIndex(int levelIndex) => 
      _uiDocument.rootVisualElement.Q<Label>(WindowNames.LevelName).text = $"LEVEL {levelIndex + 1}";

    public void SetPlayerDeathCount(int playerDeathCount) => 
      _playerDeathCountLabel.text = $"{playerDeathCount} DEATHs";

    public void SetLevelTime(float elapsedTimeInSeconds) => 
      _elapsedTimeLabel.text = $"TIME {TimeSpan.FromSeconds(elapsedTimeInSeconds):mm\\:ss\\.ff}";
  }
}
