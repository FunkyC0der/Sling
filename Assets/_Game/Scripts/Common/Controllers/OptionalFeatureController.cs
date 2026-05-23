using Playtika.Controllers;
using VContainer;

namespace Sling.Common.Controllers
{
  public class OptionalFeatureController<TController, TView> : ControllerBase
    where TController : class, IController
  {
    private readonly IObjectResolver _resolver;

    public OptionalFeatureController(IControllerFactory factory, IObjectResolver resolver)
      : base(factory)
    {
      _resolver = resolver;
    }

    protected override void OnStart()
    {
      if (_resolver.TryResolve<TView>(out _))
        Execute<TController>();
    }
  }
}
