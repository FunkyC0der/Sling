using System;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : MonoBehaviour, IView
  {
    private void OnTriggerEnter2D(Collider2D other) =>
      other?.GetComponent<IDamageable>()?.TakeDamage();    
  }
}
