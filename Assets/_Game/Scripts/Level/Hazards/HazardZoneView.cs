using System;
using Sling.Root;
using Sling.Utils;
using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : MonoBehaviour, IView
  {
    public event Action OnEnter;

    private void OnTriggerEnter2D(Collider2D other) => 
      OnEnter?.Invoke();
  }
}