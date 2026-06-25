using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class RespawnPlayerFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;
    private readonly PlayerInputView _inputView;
    private readonly PlayerAnimatorView _animatorView;
    private readonly AudioEvents _audioEvents;

    public RespawnPlayerFlowController(IControllerFactory controllerFactory,
      PlayerView view,
      LevelModel levelModel,
      PlayerInputView inputView, 
      PlayerAnimatorView animatorView, 
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
      _inputView = inputView;
      _animatorView = animatorView;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _view.SetPosition(_levelModel.PlayerStartPos);
      _view.SetFacingLeft(_levelModel.PlayerIsFacingLeft);

      _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerRevive);
      
      await _animatorView.Revive()
        .AttachExternalCancellation(cancellationToken);
      
      _view.UnfreezePhysics();
      _inputView.EnableInput();
      
      Complete();
    }
  }
}