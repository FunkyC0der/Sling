using System;
using Sling.Common;

namespace Sling.Level.Player
{
  public class PlayerModel
  {
    public Action OnPreLaunch;
    public Action OnLaunched;
    public Action OnDamaged;
    
    public readonly Observable<bool> IsInAir = new() { Value = false };
    public readonly Observable<bool> IsGrounded = new() { Value = false };
  }
}