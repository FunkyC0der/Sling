using System;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Finish
{
  [RequireComponent(typeof(Collider2D))]
  public class FinishZoneView : MonoBehaviour, IUniqueView
  {
    public event Action OnReached;

    private void OnTriggerEnter2D(Collider2D other) =>
      OnReached?.Invoke();
  }
}
