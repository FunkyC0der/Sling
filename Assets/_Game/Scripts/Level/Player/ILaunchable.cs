using System;

namespace Sling.Level.Player
{
  public interface ILaunchable
  {
    event Action OnLaunched;
  }
}