using System;
using UnityEngine;

namespace Sling.Common.Collission
{
  public class TriggerZone : MonoBehaviour
  {
    public Action<Collider2D> OnEnter;
    public Action<Collider2D> OnExit;
    
    private void OnTriggerEnter2D(Collider2D other) => 
      OnEnter?.Invoke(other);

    private void OnTriggerExit2D(Collider2D other) => 
      OnExit?.Invoke(other);
  }
}