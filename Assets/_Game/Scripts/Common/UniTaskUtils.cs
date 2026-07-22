using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Common
{
  public static class UniTaskUtils
  {
    public static async UniTask<(bool hasResultLeft, T result)> WhenAnyWithAutoCancel<T>(
      Func<CancellationToken, UniTask<T>> leftTaskFactory,
      Func<CancellationToken, UniTask> rightTaskFactory,
      CancellationToken ct)
    {
      using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

      try
      {
        return await UniTask.WhenAny(
          leftTaskFactory(linkedCts.Token),
          rightTaskFactory(linkedCts.Token));
      }
      finally
      {
        linkedCts.Cancel();
      }
    }
  }
}
