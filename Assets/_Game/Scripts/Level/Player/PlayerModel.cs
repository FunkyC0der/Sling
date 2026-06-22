using System;
using Sling.Common;

namespace Sling.Level.Player
{
  public class PlayerModel
  {
    public Action OnPreLaunch;
    public Action OnLaunched;
    public Action OnDamaged;
    
    public readonly Observable<bool> IsInAir = new();
    public readonly Observable<bool> IsGrounded = new();
    public readonly Observable<bool> IsDead = new();
    public readonly Observable<bool> IsWallSliding = new();
    public readonly Observable<bool> IsWin = new ();
  }
}