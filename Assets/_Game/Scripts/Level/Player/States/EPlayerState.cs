namespace Sling.Level.Player.States
{
  public enum EPlayerState
  {
    Spawning,
    Grounded,
    Launching,
    Airborne,
    WallSliding,
    Dead,
    Finished
  }

  public static class PlayerStates
  {
    public static bool AllowsLaunch(EPlayerState state) =>
      !IsTerminal(state);

    /// <summary>A real landing: entering Grounded from flight (not from Spawning).</summary>
    public static bool IsLanding(EPlayerState from, EPlayerState to) =>
      to == EPlayerState.Grounded &&
      from is EPlayerState.Launching or EPlayerState.Airborne or EPlayerState.WallSliding;

    public static bool IsTerminal(EPlayerState state) =>
      state is EPlayerState.Dead or EPlayerState.Finished;
  }
}
