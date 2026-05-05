using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : BaseView
  {
    public event Action OnEnter;

    private void OnTriggerEnter2D(Collider2D other) => 
      OnEnter?.Invoke();
  }
}