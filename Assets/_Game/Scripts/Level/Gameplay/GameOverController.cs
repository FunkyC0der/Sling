using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Player.Views;

namespace Sling.Level.Gameplay
{
  public class GameOverController : ControllerWithResultBase<EmptyControllerResult>
  {
    private readonly PlayerView _playerView;

    public GameOverController(IControllerFactory controllerFactory, PlayerView playerView)
      : base(controllerFactory)
    {
      _playerView = playerView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      await _playerView.Die();
      // TODO: Show game over screen.
      Complete(new EmptyControllerResult());
    }
  }
}