using System;
using Playtika.Controllers;
using Sling.Common.Extensions;
using VContainer;
using VContainer.Unity;

namespace Sling.Common.Controllers
{
  internal sealed class OwnedLifetimeScope : IDisposable
  {
    private readonly Action<IContainerBuilder> _configure;
    private readonly string _name;
    private readonly LifetimeScope _parentScope;

    private LifetimeScope _scope;

    public OwnedLifetimeScope(
      LifetimeScope parentScope,
      Action<IContainerBuilder> configure,
      string name)
    {
      _parentScope = parentScope;
      _configure = configure;
      _name = name;
    }

    public IControllerFactory ControllerFactory =>
      _scope.GetControllerFactory();

    public void Build() =>
      _scope = _parentScope.CreateChild(_configure, _name);

    public void Dispose() =>
      _scope?.Dispose();
  }
}
