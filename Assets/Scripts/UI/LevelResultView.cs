using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelResultView : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _deathPanel;

    public event Action OnRestartRequested;
    public event Action OnNextRequested;
    public event Action OnMenuRequested;

    private void Awake()
    {
        _restartButton.onClick.AddListener(() => OnRestartRequested?.Invoke());
        _nextButton.onClick.AddListener(() => OnNextRequested?.Invoke());
        _menuButton.onClick.AddListener(() => OnMenuRequested?.Invoke());
    }

    public void Show(GameplayOutcome outcome)
    {
        gameObject.SetActive(true);
        _winPanel.SetActive(outcome == GameplayOutcome.Win);
        _deathPanel.SetActive(outcome == GameplayOutcome.Death);
        _nextButton.gameObject.SetActive(outcome == GameplayOutcome.Win);
    }

    public void Hide() => gameObject.SetActive(false);
}
