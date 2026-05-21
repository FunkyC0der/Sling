using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Sling.Common.UI.Windows
{
  public delegate UniTask<TResult> WindowSessionAsync<TResult>(
    VisualElement contentRoot,
    CancellationToken cancellationToken);

  public interface IWindowRootView
  {
    UniTask<TResult> ShowAsync<TResult>(
      VisualTreeAsset contentUxml,
      WindowSessionAsync<TResult> runSession,
      CancellationToken cancellationToken);
  }
}
