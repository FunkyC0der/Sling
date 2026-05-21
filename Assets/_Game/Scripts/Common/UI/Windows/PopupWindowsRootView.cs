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
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _popupContainerTemplate;

    private VisualElement _layer;

    private void Awake() =>
      _layer = _uiDocument.rootVisualElement.Q(WindowNames.PopupLayer);

    public UniTask<TResult> ShowAsync<TResult>(
      VisualTreeAsset contentUxml,
      WindowSessionAsync<TResult> runSession,
      CancellationToken cancellationToken)
      => ShowInternalAsync(contentUxml, canClose: true, runSession, cancellationToken);

    public IWindowRootView NonSkippable() => new NonSkippableProxy(this);

    private async UniTask<TResult> ShowInternalAsync<TResult>(
      VisualTreeAsset windowTemplate,
      bool canClose,
      WindowSessionAsync<TResult> runSession,
      CancellationToken ct)
    {
      _popupContainerTemplate.CloneTree(_layer);
      VisualElement popupRoot = _layer.Children().Last();

      windowTemplate.CloneTree(popupRoot);
      VisualElement window = popupRoot.Children().First();
      
      popupRoot.pickingMode = canClose ? PickingMode.Position : PickingMode.Ignore;

      await UniTask.NextFrame(ct);
      
      popupRoot.RemoveFromClassList(WindowNames.Classes.Close);
      
      await popupRoot.WaitForTransitionEndAsync(ct);

      try
      {
        using var linkedCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
        UniTask<TResult> sessionTask = runSession(window, linkedCt.Token);

        (bool hasLeftResult, TResult result) = 
          await UniTask.WhenAny(sessionTask, popupRoot.WaitForClickAsync(linkedCt.Token));

        if (hasLeftResult)
          return result;

        linkedCt.Cancel();
        return default;
      }
      finally
      {
        popupRoot.AddToClassList(WindowNames.Classes.Close);
        
        try
        {
          await popupRoot.WaitForTransitionEndAsync(ct);
        }
        catch (System.OperationCanceledException)
        {
        }

        popupRoot.RemoveFromHierarchy();
      }
    }

    private sealed class NonSkippableProxy : IWindowRootView
    {
      private readonly PopupWindowsRootView _target;

      public NonSkippableProxy(PopupWindowsRootView target) => _target = target;

      public UniTask<TResult> ShowAsync<TResult>(
        VisualTreeAsset contentUxml,
        WindowSessionAsync<TResult> runSession,
        CancellationToken cancellationToken)
        => _target.ShowInternalAsync(contentUxml, canClose: false, runSession, cancellationToken);
    }
  }
}
