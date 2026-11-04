using System;
using Sling.Common;
using Sling.Level.Player.States;

namespace Sling.Level.Player
{
  public class PlayerModel
  {
    // --- State (written only by PlayerStateMachineController) ---
    public EPlayerState State { get; private set; } = EPlayerState.Spawning;

    /// <summary>Raised on every transition, including re-enter (from == to).</summary>
    public event Action<EPlayerState, EPlayerState> StateChanged;

    // --- Facts (written only by sensors) ---
    public readonly Observable<bool> IsGrounded = new();
    public readonly Observable<bool> IsTouchingSurface = new();
    public readonly Observable<bool> IsOnStickyWall = new();
    public event Action FinishReached;

    // --- Launch ---
    public readonly Observable<bool> IsAiming = new();
    public float AimForce;
    public int RemainingLaunches;
    public event Action PreLaunch;
    public event Action Launched;

    // --- Misc ---
    public readonly Observable<bool> IsPaused = new();

    public bool CanLaunch =>
      PlayerStates.AllowsLaunch(State) && (IsTouchingSurface.Value || RemainingLaunches > 0);

    internal void SetState(EPlayerState from, EPlayerState to)
    {
      State = to;
      StateChanged?.Invoke(from, to);
    }

    public void RaisePreLaunch() => PreLaunch?.Invoke();
    public void RaiseLaunched() => Launched?.Invoke();
    public void RaiseFinishReached() => FinishReached?.Invoke();
  }
}
