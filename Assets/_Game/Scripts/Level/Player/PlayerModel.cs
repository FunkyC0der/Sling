using System;
using Sling.Common;

namespace Sling.Level.Player
{
  public class PlayerModel
  {
    public Action OnLaunched;
    
    public readonly Observable<bool> IsInAir = new() { Value = false };
  }
}