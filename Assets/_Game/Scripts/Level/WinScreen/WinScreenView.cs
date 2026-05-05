using System;
using Sling.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Level.WinScreen
{
  [RequireComponent(typeof(UIDocument))]
  public class WinScreenView : BaseView
  {
    private static class Names
    {
      public const string kRestartButton = "restart-button";
    }

    public event Action OnRestartClicked;

    private UIDocument _document;

    private void Awake()
    {
      _document = GetComponent<UIDocument>();

      _document.enabled = false;
    }

    public void Show()
    {
      _document.enabled = true;
      _document.rootVisualElement.Q<Button>(Names.kRestartButton).clicked += OnRestartClicked;
    }

    public void Hide() => 
      _document.enabled = false;
  }
}
