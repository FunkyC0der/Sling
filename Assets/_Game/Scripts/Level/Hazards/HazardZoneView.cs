using System;
using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : MonoBehaviour
  {
    public event Action OnEnter;

    private void OnTriggerEnter2D(Collider2D other) => 
      OnEnter?.Invoke();
  }
}