using System.Collections.Generic;
using UnityEngine;

namespace Sling.Level.Elements
{
  public class GravityZone : MonoBehaviour
  {
    public GravityZoneConfig Config;
    
    private readonly List<Rigidbody2D> _rigidbodies = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(other.attachedRigidbody)
        _rigidbodies.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if(other.attachedRigidbody)
        _rigidbodies.Remove(other.attachedRigidbody);
    }

    private void FixedUpdate()
    {
      foreach (Rigidbody2D rb in _rigidbodies) 
        rb.AddForce(Config.Force * transform.up, ForceMode2D.Force);
    }
  }
}