using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Common.UI.Windows
{
  public class MenuWindowsRootView : MonoBehaviour, IWindowRootView, IUniqueView
  {
    [SerializeField] private UIDocument _uiDocument;

    private VisualElement _layer;
    private readonly Stack<VisualElement> _windowStack = new();

    private void Awake() =>
      _layer = _uiDocument.rootVisualElement.Q(WindowNames.MenuLayer);

    public VisualElement CreateAndAddWindow(VisualTreeAsset windowTemplate)
    {
      windowTemplate.CloneTree(_layer);
      VisualElement window = _layer.Children().Last();
      _windowStack.Push(window);
      return window;
    }

    public UniTask ShowAsync(CancellationToken cancellationToken) =>
      UniTask.CompletedTask;

    public UniTask<TResult> WaitForWindowResult<TResult>(
      Func<CancellationToken, UniTask<TResult>> waitForResultFunc,
      CancellationToken cancellationToken) =>
      waitForResultFunc(cancellationToken);

    public UniTask HideAsync(CancellationToken ct) =>
      UniTask.CompletedTask;

    public void RemoveWindow()
    {
      VisualElement window = _windowStack.Pop();
      _layer.Remove(window);
    }
  }
}
