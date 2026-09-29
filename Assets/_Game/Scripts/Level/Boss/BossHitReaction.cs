using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Level.Boss
{
  [Serializable]
  public abstract class BossHitReaction
  {
    public abstract UniTask Play(CancellationToken cancellationToken);

    public virtual void Stop() { }
  }
}
