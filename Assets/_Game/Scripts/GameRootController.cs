using Playtika.Controllers;

namespace Sling
{
  public class GameRootController : RootController
  {
    public GameRootController(IControllerFactory factory)
      : base(factory)
    {
    }

    protected override void OnStart()
    {
      base.OnStart();
      Execute<GameScopeController>();
    }
  }
}
