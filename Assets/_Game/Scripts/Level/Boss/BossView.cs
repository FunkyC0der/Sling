using System;
using System.Collections.Generic;
using Sling.Common.Tweeners;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Boss
{
  [Serializable]
  public class BossPhaseSettings
  {
    public PhysicsTweenerBase Tweener;
    public List<WeakPointView> WeakPoints;
  }

  public class BossView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private Transform _bossBody;
    [SerializeField] private List<BossPhaseSettings> _phases;

    public int PhaseCount => _phases.Count;

    public IReadOnlyList<WeakPointView> GetPhaseWeakPoints(int index) => _phases[index].WeakPoints;

    private int _activePhaseIndex = -1;

    public void ActivatePhase(int index)
    {
      if (index == _activePhaseIndex)
        return;

      for (int i = 0; i < _phases.Count; i++)
      {
        if (i != index)
          _phases[i].Tweener.StopTween();
      }

      PhysicsTweenerBase active = _phases[index].Tweener;
      _bossBody.SetParent(active.transform, worldPositionStays: false);
      _bossBody.localPosition = Vector3.zero;
      active.StartTween();
      _activePhaseIndex = index;
    }
  }
}
