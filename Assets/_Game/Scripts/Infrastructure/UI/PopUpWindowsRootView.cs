using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Sling.Common.Extensions;
using Sling.Common.Rendering;
using Sling.Common.UI;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Sling.Infrastructure.UI
{
  public class PopUpWindowsRootView : MonoBehaviour, IUniqueView
  {
    private class PopupWindowData
    {
      public VisualElement Background;
      public VisualElement Window;
      public bool IsBlurRequested;
      public UniTaskCompletionSource CloseRequestSource;
      public bool IsOpen;
    }

    // Matches the .container-open/.container-close transition in Popups.uss.
    private const float kBlurFadeDuration = 0.2f;

    [SerializeField] private PanelRenderer _panelRenderer;
    [SerializeField] private VisualTreeAsset _backgroundTemplate;

    private VisualElement _root;
    private readonly Dictionary<VisualElement, PopupWindowData> _popUps = new();
    // Creation order: the last one is the top pop-up that Esc closes.
    private readonly List<VisualElement> _windowsOrder = new();
    private InputAction _closeAction;
    private int _blurRequestCount;
    private Tween _blurTween;

    private void Awake()
    {
      _panelRenderer.RegisterUIReloadCallback(OnUIReloaded);

      _closeAction = new InputAction("ClosePopUp", InputActionType.Button);
      _closeAction.AddBinding("<Keyboard>/escape");
      _closeAction.AddBinding("<Gamepad>/buttonEast");
      _closeAction.performed += OnCloseActionPerformed;
      _closeAction.Enable();
    }

    private void OnDestroy()
    {
      if (_panelRenderer != null)
        _panelRenderer.UnregisterUIReloadCallback(OnUIReloaded);

      _closeAction.performed -= OnCloseActionPerformed;
      _closeAction.Dispose();

      _blurTween.Stop();
      BackdropBlur.Intensity = 0f;
    }

    private void OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
      if (_root != rootElement)
      {
        _popUps.Clear();
        _windowsOrder.Clear();
        _blurRequestCount = 0;
        _blurTween.Stop();
        BackdropBlur.Intensity = 0f;
      }

      _root = rootElement;
    }

    public VisualElement CreateWindow(VisualTreeAsset windowTemplate, bool hasBackground)
    {
      var data = new PopupWindowData();

      VisualElement windowParent = _root;

      if (hasBackground)
      {
        _backgroundTemplate.CloneTree(_root);
        data.Background = _root.Children().Last();
        windowParent = data.Background;
      }

      windowTemplate.CloneTree(windowParent);
      data.Window = windowParent.Children().Last();

      _popUps.Add(data.Window, data);
      _windowsOrder.Add(data.Window);

      return data.Window;
    }

    public async UniTask ShowWindow(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];
      data.IsOpen = true;
      UpdateCoveredWindows();

      if (data.Background != null)
      {
        data.Background.AddToClassList(WindowNames.Classes.ContainerClose);
        data.Background.AddToClassList(WindowNames.Classes.ContainerOpen);
        SetBlurRequested(data, true);
      }

      data.Window.AddToClassList(WindowNames.Classes.WindowClose);
      data.Window.AddToClassList(WindowNames.Classes.WindowOpen);

      await UniTask.NextFrame(ct);

      data.Background?.RemoveFromClassList(WindowNames.Classes.ContainerClose);
      data.Window.RemoveFromClassList(WindowNames.Classes.WindowClose);

      if (data.Background == null)
      {
        await data.Window.WaitForEventAsync<TransitionEndEvent>(ct);
        return;
      }

      await UniTask.WhenAll(
        data.Background.WaitForEventAsync<TransitionEndEvent>(ct),
        data.Window.WaitForEventAsync<TransitionEndEvent>(ct));
    }

    public async UniTask HideWindow(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];
      data.IsOpen = false;
      UpdateCoveredWindows();

      data.Background?.AddToClassList(WindowNames.Classes.ContainerClose);
      data.Window.AddToClassList(WindowNames.Classes.WindowClose);
      SetBlurRequested(data, false);

      if (data.Background == null)
      {
        await data.Window.WaitForEventAsync<TransitionEndEvent>(ct);
        return;
      }

      await UniTask.WhenAll(
        data.Background.WaitForEventAsync<TransitionEndEvent>(ct),
        data.Window.WaitForEventAsync<TransitionEndEvent>(ct));
    }

    public void RemoveWindow(VisualElement window)
    {
      if (!_popUps.TryGetValue(window, out PopupWindowData data))
        return;

      _popUps.Remove(window);
      _windowsOrder.Remove(window);
      SetBlurRequested(data, false);
      (data.Background ?? data.Window).RemoveFromHierarchy();
      UpdateCoveredWindows();
    }

    // Blurs every window under the top open pop-up with a background.
    private void UpdateCoveredWindows()
    {
      bool isCovered = false;

      for (int i = _windowsOrder.Count - 1; i >= 0; i--)
      {
        PopupWindowData data = _popUps[_windowsOrder[i]];
        data.Window.EnableInClassList(WindowNames.Classes.WindowCovered, isCovered);

        if (data.Background != null && data.IsOpen)
          isCovered = true;
      }
    }

    // The scene blur is drawn by BackdropBlurFeature; it stays on while any background window is open.
    private void SetBlurRequested(PopupWindowData data, bool isRequested)
    {
      if (data.IsBlurRequested == isRequested)
        return;

      data.IsBlurRequested = isRequested;
      _blurRequestCount += isRequested ? 1 : -1;

      float target = _blurRequestCount > 0 ? 1f : 0f;
      if (Mathf.Approximately(BackdropBlur.Intensity, target))
        return;

      _blurTween.Stop();
      _blurTween = Tween.Custom(BackdropBlur.Intensity, target, kBlurFadeDuration,
        value => BackdropBlur.Intensity = value, useUnscaledTime: true);
    }

    public UniTask GetClickOnBackgroundWaitTask(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];
      return data.Background.WaitForEventAsync<ClickEvent>(ct);
    }

    // Completes when Esc is pressed while this window is the top pop-up.
    public UniTask GetCloseRequestWaitTask(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];
      var completionSource = new UniTaskCompletionSource();
      data.CloseRequestSource = completionSource;

      ct.Register(() =>
      {
        if (data.CloseRequestSource == completionSource)
          data.CloseRequestSource = null;

        completionSource.TrySetCanceled();
      });

      return completionSource.Task;
    }

    // Click on the window background or Esc.
    public async UniTask GetDismissWaitTask(VisualElement window, CancellationToken ct)
    {
      using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

      try
      {
        await UniTask.WhenAny(
          GetClickOnBackgroundWaitTask(window, linkedCts.Token),
          GetCloseRequestWaitTask(window, linkedCts.Token));
      }
      finally
      {
        linkedCts.Cancel();
      }
    }

    private void OnCloseActionPerformed(InputAction.CallbackContext _)
    {
      if (_windowsOrder.Count == 0)
        return;

      PopupWindowData data = _popUps[_windowsOrder[^1]];
      UniTaskCompletionSource completionSource = data.CloseRequestSource;
      if (completionSource == null)
        return;

      data.CloseRequestSource = null;
      completionSource.TrySetResult();
    }
  }
}
