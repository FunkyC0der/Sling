using System;
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
    VisualElement CreateAndAddWindow(VisualTreeAsset windowTemplate);
    UniTask ShowAsync(CancellationToken cancellationToken);
    
    UniTask<TResult> WaitForWindowResult<TResult>(Func<CancellationToken, UniTask<TResult>> waitForResultFunc,
      CancellationToken cancellationToken);
    
    UniTask HideAsync(CancellationToken ct);
    void RemoveWindow();
  }
}
