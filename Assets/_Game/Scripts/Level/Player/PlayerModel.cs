
using Sling.Common;

namespace Sling.Level.Player
{
  public class PlayerModel
  {
    public readonly Observable<bool> IsInAir = new() { Value = false };
  }
}