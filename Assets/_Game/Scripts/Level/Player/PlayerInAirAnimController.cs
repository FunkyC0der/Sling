using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerInAirAnimController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly PlayerView _view;
    private readonly PlayerAnimationsView _animationsView;

    public PlayerInAirAnimController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerConfig config,
      PlayerView view,
      PlayerAnimationsView animationsView) : base(controllerFactory)
    {
      _model = model;
      _config = config;
      _view = view;
      _animationsView = animationsView;
    }

    protected override void OnStart()
    {
      FixedUpdateAsync().Forget();
      
      _model.IsGrounded.OnValueChanged += OnIsGroundedChanged;
    }

    private async UniTask FixedUpdateAsync()
    {
      while (!CancellationToken.IsCancellationRequested)
      {
        FixedUpdate();
        await UniTask.WaitForFixedUpdate(CancellationToken);
      }
    }

    private void OnIsGroundedChanged(bool oldValue, bool newValue)
    {
      if(newValue)
        _animationsView.Land();
      else
        _animationsView.InAir();
    }

    private void FixedUpdate()
    {
      if (_model.IsGrounded.Value || _model.IsDead.Value)
        return;
        
      float maxSpeedY = _config.MaxDragDistance * _config.LaunchForceMultiplier;
      float currentSpeedRatio = Mathf.Clamp(_view.LinearVelocityY / maxSpeedY, -1, 1);
      float currentSpeedRation10 = (currentSpeedRatio + 1) / 2;
      _animationsView.SetInAirBlendValue01(1 - currentSpeedRation10);
    }
  }
}