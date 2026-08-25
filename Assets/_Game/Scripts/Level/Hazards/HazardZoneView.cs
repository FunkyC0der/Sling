using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D other) =>
      other?.attachedRigidbody?.GetComponent<IDamageable>()?.TakeDamage();

    private void OnCollisionEnter2D(Collision2D other) => 
      other?.collider.attachedRigidbody?.GetComponent<IDamageable>()?.TakeDamage();
  }
}
