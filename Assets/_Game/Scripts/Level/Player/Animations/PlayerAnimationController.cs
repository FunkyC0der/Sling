using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Player.States;
using UnityEngine;

namespace Sling.Level.Player.Animations
{
  /// <summary>Maps <see cref="PlayerModel.State"/> to animations. Reacts only, never writes state.</summary>
  public class PlayerAnimationController : ControllerBase
  {
    private readonly PlayerView _view;
    private readonly PlayerAnimatorView _animatorView;
    private readonly PlayerModel _model;
    private readonly PlayerConfig _config;
    private readonly UpdateEvents _updateEvents;

    public PlayerAnimationController(IControllerFactory controllerFactory,
      PlayerView view,
      PlayerAnimatorView animatorView,
      PlayerModel model,
      PlayerConfig config,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _view = view;
      _animatorView = animatorView;
      _model = model;
      _config = config;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);

      _updateEvents.OnFixedUpdate += OnFixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= OnFixedUpdate);

      Apply(_model.State);
    }

    private void OnStateChanged(EPlayerState from, EPlayerState to) =>
      Apply(to);

    private void OnFixedUpdate()
    {
      if (_model.State != EPlayerState.Airborne)
        return;

      UpdateInAirBlend();

      // The launch clip exits to InAir by itself; switch explicitly only once it is over.
      if (!_animatorView.IsPlayingOneShot)
        _animatorView.Play(EPlayerAnimation.InAir);
    }

    private void Apply(EPlayerState state)
    {
      if (state == EPlayerState.Airborne)
      {
        UpdateInAirBlend();
        if (_animatorView.IsPlayingOneShot)
          return;
      }

      _animatorView.Play(Map(state));
    }

    private EPlayerAnimation Map(EPlayerState state) =>
      state switch
      {
        EPlayerState.Spawning => EPlayerAnimation.Idle,
        EPlayerState.Grounded => EPlayerAnimation.Idle,
        EPlayerState.Finished => EPlayerAnimation.Idle,
        EPlayerState.Airborne => EPlayerAnimation.InAir,
        EPlayerState.WallSliding => EPlayerAnimation.WallSlide,
        EPlayerState.Launching => IsStrongLaunch() ? EPlayerAnimation.LaunchStrong : EPlayerAnimation.LaunchWeak,
        _ => EPlayerAnimation.None // Dead: handled by the death flow.
      };

    private bool IsStrongLaunch() =>
      _model.AimForce / _config.GetMaxLaunchForce() > _config.StrongLaunchAnimThreshold;

    private void UpdateInAirBlend()
    {
      float maxSpeedY = _config.GetMaxLaunchForce();
      float speedRatio = Mathf.Clamp(_view.LinearVelocityY / maxSpeedY, -1, 1);
      float speedRatio01 = (speedRatio + 1) / 2;
      _animatorView.SetInAirBlend01(1 - speedRatio01);
    }
  }
}
