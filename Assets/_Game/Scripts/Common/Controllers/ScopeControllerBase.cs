using Playtika.Controllers;
using VContainer;
using VContainer.Unity;

namespace Sling.Common.Controllers
{
  public abstract class ScopeControllerBase : ControllerBase
  {
    private OwnedLifetimeScope _ownedScope;

    private readonly LifetimeScope _scope;

    protected ScopeControllerBase(IControllerFactory controllerFactory, LifetimeScope scope) 
      : base(controllerFactory)
    {
      _scope = scope;
    }

    protected abstract string OwnedScopeName { get; }
    
    protected override void OnStart() => 
      _ownedScope = BuildOwnedScope();

    protected override void OnStop() => 
      _ownedScope.Dispose();

    protected IControllerFactory GetOwnedControllerFactory() =>
      _ownedScope.ControllerFactory;

    protected abstract void InitScopeBuilder(IContainerBuilder builder);

    private OwnedLifetimeScope BuildOwnedScope()
    {
      var ownedScope = new OwnedLifetimeScope(_scope, InitScopeBuilder, OwnedScopeName);
      ownedScope.Build();
      return ownedScope;
    }
  }
}
