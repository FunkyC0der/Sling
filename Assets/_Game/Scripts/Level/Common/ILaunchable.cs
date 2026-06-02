using System;

namespace Sling.Level.Common
{
  public interface ILaunchable
  {
    event Action OnLaunched;
  }
}