using System;
using System.Collections.Generic;
using Sling.Audio;
using Sling.Level.Common;
using UnityEngine;

namespace Sling.Level.Elements.StickyWall
{
  public class StickyWall : MonoBehaviour
  {
    [field: SerializeField] public StickyWallConfig Config { get; private set; }
    [SerializeField] private AudioClipEmitter _stickClipEmitter;

    private readonly HashSet<Rigidbody2D> _collidedRbs = new();
    private readonly Dictionary<ILaunchable, Action> _launchSubscriptions = new();

    private void OnCollisionEnter2D(Collision2D other)
    {
      if (other.rigidbody)
        AddRigidbody(other.rigidbody);     
    }

    private void OnCollisionExit2D(Collision2D other)
    {
      if (other.rigidbody)
        RemoveRigidbody(other.rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(other.attachedRigidbody)
        AddRigidbody(other.attachedRigidbody);     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if(other.attachedRigidbody)
        RemoveRigidbody(other.attachedRigidbody);     
    }

    private void AddRigidbody(Rigidbody2D rb)
    {
      if (!_collidedRbs.Add(rb))
        return;

      _stickClipEmitter.PlayOneShot();
      
      var launchable = rb.GetComponent<ILaunchable>();
      if (launchable != null)
      {
        Action onLaunched = () =>
        {
          RemoveRigidbody(rb);
        };

        _launchSubscriptions.Add(launchable, onLaunched);
        launchable.OnLaunched += onLaunched;
      }
    }

    private void RemoveRigidbody(Rigidbody2D rb)
    {
      _collidedRbs.Remove(rb);

      var launchable = rb.GetComponent<ILaunchable>();
      if (launchable == null)
        return;

      if (!_launchSubscriptions.TryGetValue(launchable, out Action onLaunched))
        return;

      launchable.OnLaunched -= onLaunched;
      _launchSubscriptions.Remove(launchable);
    }

    private void FixedUpdate()
    {
      foreach (Rigidbody2D rb in _collidedRbs)
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, Config.MaxSpeed);
    }

    private void OnDisable()
    {
      foreach (KeyValuePair<ILaunchable, Action> subscription in _launchSubscriptions)
        subscription.Key.OnLaunched -= subscription.Value;

      _launchSubscriptions.Clear();
      _collidedRbs.Clear();
    }
  }
}
