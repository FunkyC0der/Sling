using System.Collections.Generic;
using UnityEngine;

namespace Sling.Level.Elements
{
  public class StickyWall : MonoBehaviour
  {
    [field: SerializeField] public StickyWallConfig Config { get; private set; }

    private readonly List<Rigidbody2D> _collidedRbs = new();

    private void OnCollisionEnter2D(Collision2D other)
    {
      if (!other.rigidbody)
        return;
      
      _collidedRbs.Add(other.rigidbody);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
      if (!other.rigidbody)
        return;
      
      _collidedRbs.Remove(other.rigidbody);
    }

    private void FixedUpdate()
    {
      foreach (Rigidbody2D rb in _collidedRbs) 
        rb.linearVelocityY = Mathf.Max(-Config.MaxFallSpeed, rb.linearVelocityY);
    }
  }
}
