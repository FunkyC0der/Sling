using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sling.Common.Extensions;
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
    }

    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _backgroundTemplate;

    private VisualElement _root;
    private readonly Dictionary<VisualElement, PopupWindowData> _popUps = new();

    private void Awake() =>
      _root = _uiDocument.rootVisualElement;

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
      (data.Background ?? data.Window).RemoveFromHierarchy();
    }

    public UniTask GetClickOnBackgroundWaitTask(VisualElement window, CancellationToken ct)
    {
      PopupWindowData data = _popUps[window];
      return data.Background.WaitForEventAsync<ClickEvent>(ct);
    }
  }
}
