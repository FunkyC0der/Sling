using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Views;

namespace Sling.Level.Elements.Switchers
{
  public class SwitchersController : ControllerBase
  {
    private readonly IReadOnlyList<Switcher> _views;

    public SwitchersController(
      IControllerFactory controllerFactory,
      IOptionalViewProvider optionalViewProvider)
      : base(controllerFactory)
    {
      _views = optionalViewProvider.GetAll<Switcher>();
    }

    protected override void OnStart()
    {
      foreach (Switcher view in _views)
        Execute<SwitcherController, Switcher>(view);
    }
  }
}
