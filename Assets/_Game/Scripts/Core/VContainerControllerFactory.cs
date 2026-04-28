using Playtika.Controllers;
using VContainer;

namespace Sling.Core
{
  public sealed class VContainerControllerFactory : IControllerFactory
  {
    private readonly IObjectResolver _resolver;

    public VContainerControllerFactory(IObjectResolver resolver)
    {
      _resolver = resolver;
    }

    public IController Create<T>() where T : class, IController
    {
      return _resolver.Resolve<T>();
    }
  }
}