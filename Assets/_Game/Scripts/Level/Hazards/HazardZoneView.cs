using UnityEngine;

namespace Sling.Level.Hazards
{
  public class HazardZoneView : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D other) =>
      other?.GetComponent<IDamageable>()?.TakeDamage();    
  }
}
