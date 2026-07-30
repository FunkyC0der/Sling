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
    private Label _playerLaunchCountLabel;

    private string _elapsedTimeFormat;
    private string _playerDeathCountFormat;
    private string _playerLaunchCountFormat;

    private void Awake()
    {
      VisualElement root = _uiDocument.rootVisualElement;
      
      _pauseButton = root.Q<Button>(WindowNames.PauseButton);
      _pauseButton.clicked += HandlePauseClicked;

      _elapsedTimeLabel = root.Q<Label>(WindowNames.kElapsedTime);
      _playerDeathCountLabel = root.Q<Label>(WindowNames.kPlayerDeathCount);
      _playerLaunchCountLabel = root.Q<Label>(WindowNames.kPlayerLaunchCount);

      _elapsedTimeFormat = _elapsedTimeLabel.text;
      _playerDeathCountFormat = _playerDeathCountLabel.text;
      _playerLaunchCountFormat = _playerLaunchCountLabel.text;
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
      _playerDeathCountLabel.text = string.Format(_playerDeathCountFormat, playerDeathCount);

    public void SetPlayerLaunchCount(int playerLaunchCount) =>
      _playerLaunchCountLabel.text = string.Format(_playerLaunchCountFormat, playerLaunchCount);

    public void SetLevelTime(float elapsedTimeInSeconds) =>
      _elapsedTimeLabel.text = string.Format(
        _elapsedTimeFormat,
        TimeSpan.FromSeconds(elapsedTimeInSeconds).ToString(@"mm\:ss\.ff"));
  }
}
