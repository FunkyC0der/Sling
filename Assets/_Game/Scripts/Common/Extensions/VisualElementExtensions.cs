using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Sling.Common.Extensions
{
  public static class VisualElementExtensions
  {
    public static UniTask WaitForEventAsync<TEvent>(this VisualElement element, CancellationToken cancellationToken) 
      where TEvent : EventBase<TEvent>, new()
    {
      var completionSource = new UniTaskCompletionSource();

      void OnEvent(TEvent eventData)
      {
        if (eventData.target != element)
          return;
        
        element.UnregisterCallback<TEvent>(OnEvent);
        completionSource.TrySetResult();
      }

      element.RegisterCallback<TEvent>(OnEvent);

      cancellationToken.Register(() =>
      {
        element.UnregisterCallback<TEvent>(OnEvent);
        completionSource.TrySetCanceled();
      });

      return completionSource.Task;
    }
    
    public static void CopyStyleSheetsTo(this VisualElement source, VisualElement target)
    {
      for (int i = 0; i < source.styleSheets.count; i++)
        target.styleSheets.Add(source.styleSheets[i]);
    }
  }
}
