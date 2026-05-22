using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sling.Common.Extensions;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Common.UI.Windows
{
  public class PopupWindowsRootView : MonoBehaviour, IWindowRootView, IUniqueView
  {
    private class PopupWindowData
    {
      public VisualElement Container;
      public VisualElement Window;
      public bool CanClose;
    }

    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _popupContainerTemplate;

    private VisualElement _layer;
    private readonly Stack<PopupWindowData> _popupStack = new();

    private void Awake() =>
      _layer = _uiDocument.rootVisualElement.Q(WindowNames.PopupLayer);

    public VisualElement CreateAndAddWindow(VisualTreeAsset windowTemplate) =>
      CreateAndAddWindowInternal(windowTemplate, canClose: true);

    public IWindowRootView NonSkippable() =>
      new NonSkippableProxy(this);

    public async UniTask ShowAsync(CancellationToken cancellationToken)
    {
      PopupWindowData data = _popupStack.Peek();

      data.Container.AddToClassList(WindowNames.Classes.ContainerClose);
      data.Container.AddToClassList(WindowNames.Classes.ContainerOpen);
      
      data.Window.AddToClassList(WindowNames.Classes.WindowClose);
      data.Window.AddToClassList(WindowNames.Classes.WindowOpen);
      
      await UniTask.NextFrame(cancellationToken);
      
      data.Container.RemoveFromClassList(WindowNames.Classes.ContainerClose);
      data.Window.RemoveFromClassList(WindowNames.Classes.WindowClose);

      await UniTask.WhenAll(data.Container.WaitForEventAsync<TransitionEndEvent>(cancellationToken), 
        data.Window.WaitForEventAsync<TransitionEndEvent>(cancellationToken));
    }

    public async UniTask<TResult> WaitForWindowResult<TResult>(Func<CancellationToken, UniTask<TResult>> waitForResultFunc,
      CancellationToken cancellationToken)
    {
      PopupWindowData data = _popupStack.Peek();

      using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

      UniTask waitForCloseTask = data.CanClose 
        ? data.Container.WaitForEventAsync<ClickEvent>(linkedCts.Token)
        : UniTask.Never(linkedCts.Token);
      
      (bool isLeftWin, TResult result) = 
        await UniTask.WhenAny(waitForResultFunc(linkedCts.Token), waitForCloseTask);

       linkedCts.Cancel();
       return isLeftWin ? result : default;
    }

    public async UniTask HideAsync(CancellationToken ct)
    {
      PopupWindowData data = _popupStack.Peek();
      
      data.Container.AddToClassList(WindowNames.Classes.ContainerClose);
      data.Window.AddToClassList(WindowNames.Classes.WindowClose);
      
      await UniTask.WhenAll(data.Container.WaitForEventAsync<TransitionEndEvent>(ct),
        data.Window.WaitForEventAsync<TransitionEndEvent>(ct));
    }

    public void RemoveWindow()
    {
      PopupWindowData data = _popupStack.Pop();
      _layer.Remove(data.Container);
    }

    private VisualElement CreateAndAddWindowInternal(VisualTreeAsset windowTemplate, bool canClose)
    {
      var data = new PopupWindowData { CanClose = canClose };

      _popupContainerTemplate.CloneTree(_layer);
      data.Container = _layer.Children().Last();

      windowTemplate.CloneTree(data.Container);
      data.Window = data.Container.Children().First();

      _popupStack.Push(data);

      return data.Window;
    }

    private sealed class NonSkippableProxy : IWindowRootView
    {
      private readonly PopupWindowsRootView _owner;

      public NonSkippableProxy(PopupWindowsRootView owner) =>
        _owner = owner;

      public VisualElement CreateAndAddWindow(VisualTreeAsset windowTemplate) =>
        _owner.CreateAndAddWindowInternal(windowTemplate, canClose: false);

      public UniTask ShowAsync(CancellationToken cancellationToken) =>
        _owner.ShowAsync(cancellationToken);

      public UniTask<TResult> WaitForWindowResult<TResult>(Func<CancellationToken, UniTask<TResult>> waitForResultFunc,
        CancellationToken cancellationToken) =>
        _owner.WaitForWindowResult(waitForResultFunc, cancellationToken);

      public UniTask HideAsync(CancellationToken ct) =>
        _owner.HideAsync(ct);

      public void RemoveWindow() =>
        _owner.RemoveWindow();
    }
  }
}
