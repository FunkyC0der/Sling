using System;
using UnityEngine;

namespace Sling.Common.Collission
{
  public class TriggerZone : MonoBehaviour
  {
    public Action<Collider2D> OnEnter;
    public Action<Collider2D> OnExit;
    public Action<Collision2D> OnCollideEnter;
    public Action<Collision2D> OnCollideExit;
    
    private void OnTriggerEnter2D(Collider2D other) => 
      OnEnter?.Invoke(other);

    private void OnTriggerExit2D(Collider2D other) => 
      OnExit?.Invoke(other);

    private void OnCollisionEnter2D(Collision2D other) => 
      OnCollideEnter?.Invoke(other);

    private void OnCollisionExit2D(Collision2D other) => 
      OnCollideExit?.Invoke(other);

    public bool IsColliding() =>
      GetComponent<Collider2D>().IsTouchingLayers();
  }
}