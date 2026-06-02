using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Collision
{
  public class CollisionTriggerView : MonoBehaviour, IGameObjectView
  {
    public readonly CollisionEvents Events = new();

    private void OnCollisionEnter2D(Collision2D other) => 
      Events.OnEnter?.Invoke(other);

    private void OnCollisionExit2D(Collision2D other) => 
      Events.OnExit?.Invoke(other);

    private void OnTriggerEnter2D(Collider2D other) => 
      Events.OnTriggerEnter?.Invoke(other);

    private void OnTriggerExit2D(Collider2D other) => 
      Events.OnTriggerExit?.Invoke(other);
  }
}