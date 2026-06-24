using Playtika.Controllers;
using Sling.Common.Views;
using VContainer;

namespace Sling.Common.Controllers
{
  public class TryGetViewFlowController<TView> : ControllerWithResultBase<TView> where TView : IView
  {
    private readonly IObjectResolver _objectResolver;

    public TryGetViewFlowController(IControllerFactory controllerFactory, IObjectResolver objectResolver)
      : base(controllerFactory)
    {
      _objectResolver = objectResolver;
    }

    protected override void OnStart()
    {
      if(_objectResolver.TryResolve(out TView view))
        Complete(view);
      
      Complete(default);
    }
  }
}