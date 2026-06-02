using System;
using UnityEngine;

namespace Sling.Level.Collision
{
  public class CollisionEvents
  {
    public Action<Collision2D> OnEnter;
    public Action<Collision2D> OnExit;
    
    public Action<Collider2D> OnTriggerEnter;
    public Action<Collider2D> OnTriggerExit;    
  }
}