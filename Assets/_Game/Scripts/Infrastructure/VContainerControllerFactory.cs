using Playtika.Controllers;
using VContainer;
using VContainer.Unity;

namespace Sling.Infrastructure
{
  public sealed class VContainerControllerFactory : IControllerFactory
  {
    private readonly IObjectResolver _resolver;
    private readonly string _scopeName;

    public VContainerControllerFactory(IObjectResolver resolver, LifetimeScope scope)
    {
      _resolver = resolver;
      _scopeName = scope.name;
    }

    public IController Create<T>() where T : class, IController => 
      _resolver.Resolve<T>();

    public override string ToString() => 
      _scopeName;
  }
}