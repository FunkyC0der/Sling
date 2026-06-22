using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerAnimatorController : ControllerBase
  {
    private const float _kVelocityThreshold = 0.2f;
    
    private enum EState
    {
      Idle,
      InAir,
      WallSliding,
      Dead
    }
    
    private EState _state;
    
    private readonly PlayerView _view;
    private readonly PlayerAnimatorView _animatorView;
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly UpdateEvents _updateEvents;

    public PlayerAnimatorController(IControllerFactory controllerFactory,
      PlayerView view,
      PlayerAnimatorView animatorView,
      PlayerModel model, 
      PlayerConfig config,
      UpdateEvents updateEvents) : base(controllerFactory)
    {
      _view = view;
      _animatorView = animatorView;
      _model = model;
      _config = config;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnFixedUpdate += FixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= FixedUpdate);
      
      _model.IsWin.OnValueChanged += OnIsWinChanged;
      this.AddDisposableAction(() => _model.IsWin.OnValueChanged -= OnIsWinChanged);
      
      _state = EState.Idle;
      EnterState();
    }

    private void ChangeState(EState newState)
    {
      if(_state == newState)
        return;
      
      _state = newState;
      EnterState();
    }

    private void EnterState()
    {
      switch (_state)
      {
        case EState.Idle:
          _animatorView.Land();
          break;
        
        case EState.InAir:
          _animatorView.InAir();
          break;
        
        case EState.WallSliding:
          _animatorView.WallSlide();
          break;
      }
    }

    private void OnIsWinChanged(bool oldValue, bool isWin)
    {
      if (isWin)
        ChangeState(EState.Idle);
    }

    private void FixedUpdate()
    {
      if(_model.IsDead.Value)
        ChangeState(EState.Dead);
      
      switch (_state)
      {
        case EState.Idle:
          if(!_model.IsGrounded.Value)
            ChangeState(EState.InAir);
          break;

        case EState.InAir:
        {
          if (_model.IsGrounded.Value)
          {
            ChangeState(EState.Idle);
            break;
          }

          if (_model.IsWallSliding.Value)
          {
            ChangeState(EState.WallSliding);
            break;
          }
          
          if (_view.LinearVelocityX > _kVelocityThreshold)
            _view.SetFacingLeft(false);
          else if (_view.LinearVelocityX < -_kVelocityThreshold)
            _view.SetFacingLeft(true);
          
          float maxSpeedY = _config.MaxDragDistance * _config.LaunchForceMultiplier;
          float currentSpeedRatio = Mathf.Clamp(_view.LinearVelocityY / maxSpeedY, -1, 1);
          float currentSpeedRation10 = (currentSpeedRatio + 1) / 2;
          _animatorView.SetInAirBlendValue01(1 - currentSpeedRation10);
          
          break;
        }
        case EState.WallSliding:
          if(!_model.IsWallSliding.Value)
            ChangeState(_model.IsGrounded.Value ? EState.Idle : EState.InAir);
          break;
      }
    }
  }
}
