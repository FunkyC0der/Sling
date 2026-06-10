using Playtika.Controllers;
using Sling.Level.Player;
using Sling.Level.Session;

namespace Sling.Level.Gameplay
{
  public class RespawnPlayerFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;
    private readonly PlayerInputView _inputView;

    public RespawnPlayerFlowController(IControllerFactory controllerFactory,
      PlayerView view,
      LevelModel levelModel,
      PlayerInputView inputView)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
      _inputView = inputView;
    }

    protected override void OnStart()
    {
      _view.SetPosition(_levelModel.PlayerStartPos);
      _view.SetFacingLeft(_levelModel.PlayerIsFacingLeft);
      
      _view.UnfreezePhysics();
      _view.Show();

      _inputView.EnableInput();
      
      Complete();
    }
  }
}