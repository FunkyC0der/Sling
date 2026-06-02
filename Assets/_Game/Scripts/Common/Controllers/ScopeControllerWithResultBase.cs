using Playtika.Controllers;
using Sling.Common.Extensions;
using VContainer;
using VContainer.Unity;

namespace Sling.Common.Controllers
{
  public abstract class ScopeControllerWithResultBase<TResult> : ControllerWithResultBase<TResult>
  {
    private LifetimeScope _ownedScope;

    private readonly LifetimeScope _scope;

    protected ScopeControllerWithResultBase(IControllerFactory controllerFactory, LifetimeScope scope)
      : base(controllerFactory)
    {
      _scope = scope;
    }

    protected override void OnStart() =>
      _ownedScope = BuildOwnedScope();

    protected override void OnStop() =>
      _ownedScope.Dispose();

    protected IControllerFactory GetOwnedControllerFactory() =>
      _ownedScope.GetControllerFactory();

    protected abstract void InitScopeBuilder(IContainerBuilder builder);

    private LifetimeScope BuildOwnedScope() =>
      _scope.CreateChild(InitScopeBuilder);
  }
}
