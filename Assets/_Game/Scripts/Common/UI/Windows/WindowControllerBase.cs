using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine.UIElements;

namespace Sling.Common.UI.Windows
{
  public abstract class WindowControllerBase<TView, TResult>
    : ControllerWithResultBase<IWindowRootView, TResult>
  {
    protected WindowControllerBase(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    protected abstract VisualTreeAsset Uxml { get; }
    protected abstract TView CreateView(VisualElement contentRoot);
    protected abstract UniTask<TResult> AwaitResultAsync(TView view, CancellationToken cancellationToken);

    protected sealed override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      TResult result = await Args.ShowAsync<TResult>(
        Uxml,
        (contentRoot, ct) =>
        {
          TView view = CreateView(contentRoot);
          return AwaitResultAsync(view, ct);
        },
        cancellationToken);

      Complete(result);
    }
  }
}
