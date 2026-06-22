using System;
using Playtika.Controllers;

namespace Sling.Common.Extensions
{
  public static class ControllerExtensions
  {
    public static void AddDisposableAction(this IController controller, Action action) =>
      controller.AddDisposable(new DisposableToken(action));
  }
}