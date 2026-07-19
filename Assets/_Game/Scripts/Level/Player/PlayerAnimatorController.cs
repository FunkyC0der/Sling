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
      OnGround,
      PreLaunch,
      InAir,
      WallSliding,
      Dead,
      Win
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

      _model.IsDead.OnValueChanged += OnIsDeadChanged;
      this.AddDisposableAction(() => _model.IsDead.OnValueChanged -= OnIsDeadChanged);
      
      _model.IsWin.OnValueChanged += OnIsWinChanged;
      this.AddDisposableAction(() => _model.IsWin.OnValueChanged -= OnIsWinChanged);
      
      _state = EState.OnGround;
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
        case EState.OnGround:
        case EState.Win:
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

    private void FixedUpdate()
    {
      if(_model.IsDead.Value)
        ChangeState(EState.Dead);
      
      switch (_state)
      {
        case EState.OnGround:
          if(!_model.IsGrounded.Value)
          {
            ChangeState(EState.InAir);
            break;
          }
          
          UpdateFacingDirection();
          break;

        case EState.InAir:
        {
          if (_model.IsGrounded.Value)
          {
            ChangeState(EState.OnGround);
            break;
          }

          if (_model.IsWallSliding.Value)
          {
            ChangeState(EState.WallSliding);
            break;
          }
          
          UpdateFacingDirection();
          
          float maxSpeedY = _config.MaxDragDistance * _config.LaunchForceMultiplier;
          float currentSpeedRatio = Mathf.Clamp(_view.LinearVelocityY / maxSpeedY, -1, 1);
          float currentSpeedRation10 = (currentSpeedRatio + 1) / 2;
          _animatorView.SetInAirBlendValue01(1 - currentSpeedRation10);
          
          break;
        }
        
        case EState.WallSliding:
          if(!_model.IsWallSliding.Value)
            ChangeState(_model.IsGrounded.Value ? EState.OnGround : EState.InAir);
          break;
      }
    }

    private void UpdateFacingDirection()
    {
      if (_view.LinearVelocityX > _kVelocityThreshold)
        _view.SetFacingLeft(false);
      else if (_view.LinearVelocityX < -_kVelocityThreshold)
        _view.SetFacingLeft(true);
    }

    private void OnIsDeadChanged(bool oldValue, bool newValue)
    {
      if(newValue)
        ChangeState(EState.Dead);
    }

    private void OnIsWinChanged(bool oldValue, bool newValue)
    {
      if (newValue)
        ChangeState(EState.Win);
    }
  }
}
