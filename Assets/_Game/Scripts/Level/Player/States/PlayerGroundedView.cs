using System;
using Sling.Common.Collission;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player.States
{
  public class PlayerGroundedView : MonoBehaviour, IGameObjectView
  {
    [SerializeField] private TriggerZone _triggerZone;
    
    public Action<Collider2D> OnGrounded
    {
      get => _triggerZone.OnEnter;
      set => _triggerZone.OnEnter = value;
    }

    public Action<Collider2D> OnUngrounded
    {
      get => _triggerZone.OnExit;
      set => _triggerZone.OnExit = value;
    }
  }
}