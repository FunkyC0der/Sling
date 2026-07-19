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
      Invalid,
      OnGround,
      Launch,
      InAir,
      WallSliding,
      Dead,
      Win
    }
    
    private EState _state = EState.Invalid;
    
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

      _model.OnLaunched += OnLaunched;
      
      ChangeState(EState.OnGround);
    }

    private void ChangeState(EState newState)
    {
      if(_state == newState)
        return;
      
      ExitState();

      EState prevState = _state;
      _state = newState;
      
      EnterState(prevState);
    }

    private void EnterState(EState prevState)
    {
      switch (_state)
      {
        case EState.OnGround:
          _animatorView.Idle();
          break;
        
        case EState.Win:
          _animatorView.Idle();
          break;
        
        case EState.InAir:
          if(prevState != EState.Launch)
            _animatorView.InAir();
          
          break;
        
        case EState.WallSliding:
          _animatorView.WallSlide();
          break;
      }
    }

    private void ExitState()
    {
      switch (_state)
      {
        case EState.OnGround:
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
            UpdateLaunchForceBlendValue();
            ChangeState(EState.InAir);
            break;
          }
          
          UpdateFacingDirection();
          break;

        case EState.Launch:
          UpdateLaunchForceBlendValue();
          _animatorView.Launch();
          
          ChangeState(EState.InAir);
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
          UpdateInAirBlendValue();

          break;
        }
        
        case EState.WallSliding:
          if(!_model.IsWallSliding.Value)
            ChangeState(_model.IsGrounded.Value ? EState.OnGround : EState.InAir);
          break;
      }
    }

    private void UpdateInAirBlendValue()
    {
      float maxSpeedY = _config.GetMaxLaunchForce();
      float currentSpeedRatio = Mathf.Clamp(_view.LinearVelocityY / maxSpeedY, -1, 1);
      float currentSpeedRation10 = (currentSpeedRatio + 1) / 2;
      _animatorView.SetInAirBlendValue01(1 - currentSpeedRation10);
    }

    private void UpdateLaunchForceBlendValue() => 
      _animatorView.SetLaunchForceBlend01(_model.PreLaunchForce / _config.GetMaxLaunchForce());

    private void UpdateFacingDirection()
    {
      if (_view.LinearVelocityX > _kVelocityThreshold)
        _view.SetFacingLeft(false);
      else if (_view.LinearVelocityX < -_kVelocityThreshold)
        _view.SetFacingLeft(true);
    }

    private void OnLaunched() => 
      ChangeState(EState.Launch);

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
