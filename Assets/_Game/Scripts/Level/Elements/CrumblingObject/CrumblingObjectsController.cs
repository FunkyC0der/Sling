using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Views;

namespace Sling.Level.Elements.CrumblingObject
{
  public class CrumblingObjectsController : ControllerBase
  {
    private readonly IReadOnlyList<CrumblingObject> _views;

    public CrumblingObjectsController(
      IControllerFactory controllerFactory,
      IOptionalViewProvider optionalViewProvider)
      : base(controllerFactory)
    {
      _views = optionalViewProvider.GetAll<CrumblingObject>();
    }

    protected override void OnStart()
    {
      foreach (CrumblingObject view in _views)
        Execute<CrumblingObjectController, CrumblingObject>(view);
    }
  }
}
