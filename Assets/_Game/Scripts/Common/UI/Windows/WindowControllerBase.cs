using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine.UIElements;

namespace Sling.Common.UI.Windows
{
  public abstract class WindowControllerBase<TResult> : ControllerWithResultBase<IWindowRootView, TResult>
  {
    protected WindowControllerBase(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    protected abstract VisualTreeAsset WindowTemplate { get; }
    protected abstract void InitWindow(VisualElement window);
    protected abstract UniTask<TResult> WaitForResult(CancellationToken cancellationToken);

    private VisualElement _window;

    protected sealed override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _window = Args.CreateAndAddWindow(WindowTemplate);

      try
      {
        InitWindow(_window);
        await Args.ShowAsync(cancellationToken);
        TResult result = await Args.WaitForWindowResult(WaitForResult, cancellationToken);
        await Args.HideAsync(cancellationToken);

        Complete(result);
      }
      finally
      {
        Args.RemoveWindow();
      }
    }
  }
}
