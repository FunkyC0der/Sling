using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sling.Common.Tweeners;
using UnityEngine;

namespace Sling.Level.Boss
{
  [Serializable]
  public class BossPhaseSettings
  {
    public PhysicsTweenerBase Tweener;
    public List<WeakPointView> WeakPoints;
    
    public Vector3 InitialPosition { get; set; }

    public void Start() => 
      Tweener.StartTween();

    public void Stop() =>
      Tweener.StopTween();

    public void SaveInitialTransform() => 
      InitialPosition = Tweener.transform.position;

    public void ResetToInitialTransform() => 
      Tweener.Rigidbody.position = InitialPosition;

    public async UniTask ShowWeakPointsAnim(CancellationToken cancellationToken)
    {
      List<UniTask> animations = new();
        
      foreach (WeakPointView weakPoint in WeakPoints)
      {
        if(!weakPoint.IsHidden)
          continue;
        
        UniTask anim = weakPoint.Show(cancellationToken);
        animations.Add(anim);
      }

      await UniTask.WhenAll(animations);
    }

    public async UniTask HideWeakPointsAnim(CancellationToken cancellationToken)
    {
      List<UniTask> animations = new();
        
      foreach (WeakPointView weakPoint in WeakPoints)
      {
        if(weakPoint.IsHidden)
          continue;
        
        UniTask anim = weakPoint.HideWithAnim(cancellationToken);
        animations.Add(anim);
      }

      await UniTask.WhenAll(animations);
    }

    public void AttachBossBody(Transform bossBody) => 
      bossBody.SetParent(Tweener.transform, worldPositionStays: false);
  }
}