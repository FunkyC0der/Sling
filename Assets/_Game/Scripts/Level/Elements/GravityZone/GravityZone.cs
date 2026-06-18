using System.Collections.Generic;
using UnityEngine;

namespace Sling.Level.Elements.GravityZone
{
  public class GravityZone : MonoBehaviour
  {
    public GravityZoneConfig Config;
    
    private readonly HashSet<Rigidbody2D> _rigidbodies = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(other.attachedRigidbody)
      {
        if(_rigidbodies.Add(other.attachedRigidbody))
          other.attachedRigidbody.gravityScale *= Config.GravityScaleMultiplier;
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if(other.attachedRigidbody)
      {
        if (_rigidbodies.Remove(other.attachedRigidbody)) 
          other.attachedRigidbody.gravityScale /= Config.GravityScaleMultiplier;
      }
    }

    private void FixedUpdate()
    {
      foreach (Rigidbody2D rb in _rigidbodies) 
        rb.AddForce(Config.Force * transform.up, ForceMode2D.Force);
    }
  }
}