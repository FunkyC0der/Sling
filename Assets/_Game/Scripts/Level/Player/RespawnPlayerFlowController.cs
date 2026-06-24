using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class RespawnPlayerFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;
    private readonly PlayerInputView _inputView;
    private readonly PlayerAnimatorView _animatorView;

    public RespawnPlayerFlowController(IControllerFactory controllerFactory,
      PlayerView view,
      LevelModel levelModel,
      PlayerInputView inputView, 
      PlayerAnimatorView animatorView)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
      _inputView = inputView;
      _animatorView = animatorView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _view.SetPosition(_levelModel.PlayerStartPos);
      _view.SetFacingLeft(_levelModel.PlayerIsFacingLeft);

      await _animatorView.Revive()
        .AttachExternalCancellation(cancellationToken);
      
      _view.UnfreezePhysics();
      _inputView.EnableInput();
      
      Complete();
    }
  }
}