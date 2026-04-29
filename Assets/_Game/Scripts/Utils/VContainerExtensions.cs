using Playtika.Controllers;
using VContainer;
using VContainer.Unity;

namespace Sling.Utils
{
  public static class VContainerExtensions
  {
    public static IControllerFactory GetControllerFactory(this LifetimeScope scope) =>
      scope.Container.Resolve<IControllerFactory>();
  }
}