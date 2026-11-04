using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Hazards;
using UnityEngine;

namespace Sling.Level.Player.States
{
  /// <summary>The only writer of <see cref="PlayerModel.State"/>.</summary>
  public class PlayerStateMachineController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly UpdateEvents _updateEvents;
    private readonly DamageableView _damageableView;
    private readonly Dictionary<EPlayerState, PlayerStateBase> _states;

    private PlayerStateBase _current;

    public PlayerStateMachineController(IControllerFactory controllerFactory,
      PlayerModel model,
      PlayerConfig config,
      UpdateEvents updateEvents,
      DamageableView damageableView)
      : base(controllerFactory)
    {
      _model = model;
      _updateEvents = updateEvents;
      _damageableView = damageableView;

      _states = new Dictionary<EPlayerState, PlayerStateBase>();
      Add(new SpawningState(model, config));
      Add(new GroundedState(model, config));
      Add(new LaunchingState(model, config));
      Add(new AirborneState(model, config));
      Add(new WallSlidingState(model, config));
      Add(new TerminalState(EPlayerState.Dead, model, config));
      Add(new TerminalState(EPlayerState.Finished, model, config));
    }

    protected override void OnStart()
    {
      _current = _states[EPlayerState.Spawning];
      _current.Enter();
      // PlayerModel starts in Spawning; the scope (and the model) is recreated every life.

      _updateEvents.OnFixedUpdate += OnFixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= OnFixedUpdate);

      _damageableView.OnDamaged += OnDamaged;
      this.AddDisposableAction(() => _damageableView.OnDamaged -= OnDamaged);

      _model.Launched += OnLaunched;
      this.AddDisposableAction(() => _model.Launched -= OnLaunched);

      _model.FinishReached += OnFinishReached;
      this.AddDisposableAction(() => _model.FinishReached -= OnFinishReached);
    }

    private void Add(PlayerStateBase state) =>
      _states.Add(state.Id, state);

    private bool IsTerminal => PlayerStates.IsTerminal(_current.Id);

    private void OnFixedUpdate()
    {
      if (IsTerminal)
        return;

      EPlayerState next = _current.FixedTick(Time.fixedDeltaTime);
      ChangeState(next, allowReenter: false);
    }

    // Handled immediately (not on the next FixedUpdate) so the launch anim starts the same frame.
    private void OnLaunched()
    {
      if (!_current.AllowsLaunch)
        return;

      ChangeState(EPlayerState.Launching, allowReenter: true);
    }

    private void OnDamaged()
    {
      if (!IsTerminal)
        ChangeState(EPlayerState.Dead, allowReenter: false);
    }

    private void OnFinishReached()
    {
      if (!IsTerminal)
        ChangeState(EPlayerState.Finished, allowReenter: false);
    }

    private void ChangeState(EPlayerState id, bool allowReenter)
    {
      if (id == _current.Id && !allowReenter)
        return;

      EPlayerState from = _current.Id;
      _current.Exit();
      _current = _states[id];
      _current.Enter();

      _model.SetState(from, id);
    }
  }
}
