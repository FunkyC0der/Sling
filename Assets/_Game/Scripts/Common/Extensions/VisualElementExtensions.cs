using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Sling.Common.Extensions
{
  public static class VisualElementExtensions
  {
    public static UniTask WaitForTransitionEndAsync(
      this VisualElement element,
      CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource();

      void OnTransitionEnd(TransitionEndEvent _)
      {
        element.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
        completionSource.TrySetResult();
      }

      element.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);

      cancellationToken.Register(() =>
      {
        element.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
        completionSource.TrySetCanceled();
      });

      return completionSource.Task;
    }

    public static UniTask WaitForClickAsync(
      this VisualElement element,
      CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource();

      void OnClick(ClickEvent _)
      {
        element.UnregisterCallback<ClickEvent>(OnClick);
        completionSource.TrySetResult();
      }

      element.RegisterCallback<ClickEvent>(OnClick);

      cancellationToken.Register(() =>
      {
        element.UnregisterCallback<ClickEvent>(OnClick);
        completionSource.TrySetCanceled();
      });

      return completionSource.Task;
    }
  }
}
