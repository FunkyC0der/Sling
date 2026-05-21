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

    private void Awake() =>
      _layer = _uiDocument.rootVisualElement.Q(WindowNames.MenuLayer);

    public async UniTask<TResult> ShowAsync<TResult>(
      VisualTreeAsset contentUxml,
      WindowSessionAsync<TResult> runSession,
      CancellationToken cancellationToken)
    {
      VisualElement contentRoot = contentUxml.Instantiate();
      _layer.Add(contentRoot);
      try
      {
        return await runSession(contentRoot, cancellationToken);
      }
      finally
      {
        contentRoot.RemoveFromHierarchy();
      }
    }
  }
}
