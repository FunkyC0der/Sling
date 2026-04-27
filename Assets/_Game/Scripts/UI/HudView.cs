using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sling.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;

        public event Action OnRestartRequested;

        private void Awake()
        {
            _restartButton.onClick.AddListener(() => OnRestartRequested?.Invoke());
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
