using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Views;

namespace Sling.Level.Elements.MovingLevelPart
{
  public class MovingLevelPartsController : ControllerBase
  {
    private readonly IReadOnlyList<MovingLevelPartView> _views;

    public MovingLevelPartsController(
      IControllerFactory controllerFactory,
      IOptionalViewProvider optionalViewProvider)
      : base(controllerFactory)
    {
      _views = optionalViewProvider.GetAll<MovingLevelPartView>();
    }

    protected override void OnStart()
    {
      foreach (MovingLevelPartView view in _views)
        Execute<MovingLevelPartController, MovingLevelPartView>(view);
    }
  }
}
