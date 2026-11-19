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
    }

    // Matches the .container-open/.container-close transition in Popups.uss.
    private const float kBlurFadeDuration = 0.2f;

    [SerializeField] private PanelRenderer _panelRenderer;
    [SerializeField] private VisualTreeAsset _backgroundTemplate;

    private VisualElement _root;
    private readonly Dictionary<VisualElement, PopupWindowData> _popUps = new();
    private int _blurRequestCount;
    private Tween _blurTween;

    private void Awake() =>
      _panelRenderer.RegisterUIReloadCallback(OnUIReloaded);

    private void OnDestroy()
    {
      if (_panelRenderer != null)
        _panelRenderer.UnregisterUIReloadCallback(OnUIReloaded);

      _blurTween.Stop();
      BackdropBlur.Intensity = 0f;
    }

    private void OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
      if (_root != rootElement)
      {
        _popUps.Clear();
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

      return data.Window;
    }

    public async UniTask ShowWindow(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];

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
      SetBlurRequested(data, false);
      (data.Background ?? data.Window).RemoveFromHierarchy();
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
  }
}
