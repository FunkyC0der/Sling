using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Audio;
using Sling.Level.Player.Death;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  public class RespawnPlayerFlowController : ControllerWithResultBase
  {
    private readonly PlayerView _view;
    private readonly LevelModel _levelModel;
    private readonly PlayerDeathView _deathView;
    private readonly AudioEvents _audioEvents;

    public RespawnPlayerFlowController(IControllerFactory controllerFactory,
      PlayerView view,
      LevelModel levelModel,
      PlayerDeathView deathView,
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _view = view;
      _levelModel = levelModel;
      _deathView = deathView;
      _audioEvents = audioEvents;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      _view.SetPosition(_levelModel.PlayerStartPos);
      _view.SetFacingLeft(_levelModel.PlayerIsFacingLeft);

      _audioEvents.PlaySFX?.Invoke(AudioClipId.PlayerRevive);
      
      await _deathView.Revive()
        .AttachExternalCancellation(cancellationToken);
      
      // Input is enabled by PlayerInputGateController, which starts after this flow.
      _view.UnfreezePhysics();
      
      Complete();
    }
  }
}