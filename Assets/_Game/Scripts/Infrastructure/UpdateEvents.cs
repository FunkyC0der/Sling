using System;

namespace Sling.Infrastructure
{
  public class UpdateEvents
  {
    public Action OnUpdate;
    public Action OnPostLateUpdate;
    public Action OnFixedUpdate;
  }
}