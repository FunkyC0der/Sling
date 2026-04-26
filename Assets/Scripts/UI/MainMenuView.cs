using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _playButton;

    public event Action OnPlayRequested;

    private void Awake()
    {
        _playButton.onClick.AddListener(() => OnPlayRequested?.Invoke());
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
