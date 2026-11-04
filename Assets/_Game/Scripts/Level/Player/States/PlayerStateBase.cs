namespace Sling.Level.Player.States
{
  /// <summary>
  /// Plain FSM state owned and created by <see cref="PlayerStateMachineController"/>. Not a controller.
  /// </summary>
  public abstract class PlayerStateBase
  {
    protected readonly PlayerModel Model;
    protected readonly PlayerConfig Config;

    protected PlayerStateBase(PlayerModel model, PlayerConfig config)
    {
      Model = model;
      Config = config;
    }

    public abstract EPlayerState Id { get; }
    public bool AllowsLaunch => PlayerStates.AllowsLaunch(Id);

    public virtual void Enter() { }
    public virtual void Exit() { }

    /// <summary>Returns the next state, or own <see cref="Id"/> to stay.</summary>
    public virtual EPlayerState FixedTick(float dt) => Id;
  }

  public sealed class SpawningState : PlayerStateBase
  {
    private float _elapsed;

    public SpawningState(PlayerModel model, PlayerConfig config) : base(model, config) { }

    public override EPlayerState Id => EPlayerState.Spawning;

    public override void Enter() => _elapsed = 0;

    public override EPlayerState FixedTick(float dt)
    {
      _elapsed += dt;

      if (Model.IsGrounded.Value)
        return EPlayerState.Grounded;

      return _elapsed >= Config.SpawnSettleTime ? EPlayerState.Airborne : Id;
    }
  }

  public sealed class GroundedState : PlayerStateBase
  {
    public GroundedState(PlayerModel model, PlayerConfig config) : base(model, config) { }

    public override EPlayerState Id => EPlayerState.Grounded;

    public override EPlayerState FixedTick(float dt) =>
      Model.IsGrounded.Value ? Id : EPlayerState.Airborne;
  }

  public sealed class LaunchingState : PlayerStateBase
  {
    private float _elapsed;

    public LaunchingState(PlayerModel model, PlayerConfig config) : base(model, config) { }

    public override EPlayerState Id => EPlayerState.Launching;

    public override void Enter() => _elapsed = 0;

    public override EPlayerState FixedTick(float dt)
    {
      _elapsed += dt;
      if (_elapsed < Config.LaunchGroundIgnoreTime)
        return Id;

      if (Model.IsOnStickyWall.Value)
        return EPlayerState.WallSliding;

      return Model.IsGrounded.Value ? EPlayerState.Grounded : EPlayerState.Airborne;
    }
  }

  public sealed class AirborneState : PlayerStateBase
  {
    public AirborneState(PlayerModel model, PlayerConfig config) : base(model, config) { }

    public override EPlayerState Id => EPlayerState.Airborne;

    public override EPlayerState FixedTick(float dt)
    {
      if (Model.IsGrounded.Value)
        return EPlayerState.Grounded;

      return Model.IsOnStickyWall.Value ? EPlayerState.WallSliding : Id;
    }
  }

  public sealed class WallSlidingState : PlayerStateBase
  {
    public WallSlidingState(PlayerModel model, PlayerConfig config) : base(model, config) { }

    public override EPlayerState Id => EPlayerState.WallSliding;

    public override EPlayerState FixedTick(float dt)
    {
      if (Model.IsOnStickyWall.Value)
        return Id;

      return Model.IsGrounded.Value ? EPlayerState.Grounded : EPlayerState.Airborne;
    }
  }

  /// <summary>Dead / Finished: ignores everything.</summary>
  public sealed class TerminalState : PlayerStateBase
  {
    public TerminalState(EPlayerState id, PlayerModel model, PlayerConfig config) : base(model, config) =>
      Id = id;

    public override EPlayerState Id { get; }
  }
}
