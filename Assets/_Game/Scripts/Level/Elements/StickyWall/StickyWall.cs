using System;
using System.Collections.Generic;
using Sling.Audio;
using Sling.Common.Collission;
using Sling.Level.Common;
using UnityEngine;

namespace Sling.Level.Elements.StickyWall
{
  public class StickyWall : MonoBehaviour
  {
    [field: SerializeField] public StickyWallConfig Config { get; private set; }
    [SerializeField] private AudioClipEmitter _stickClipEmitter;
    
    [SerializeField] private TriggerZone _slowdownTriggerZone;
    [SerializeField] private TriggerZone _applyPhysMaterialTriggerZone;

    private readonly HashSet<Rigidbody2D> _collidedRbs = new();
    private readonly Dictionary<ILaunchable, Action> _launchSubscriptions = new();
    private readonly Dictionary<Collider2D, PhysicsMaterial2D> _origPhysMaterials = new();

    private void Awake()
    {
      _slowdownTriggerZone.OnEnter += OnEnterSlowdownZone;
      _slowdownTriggerZone.OnExit += OnExitSlowdownZone;
      
      _applyPhysMaterialTriggerZone.OnEnter += ApplyPhysMaterial;
      _applyPhysMaterialTriggerZone.OnExit += ResetPhysMaterial;
    }

    private void OnEnterSlowdownZone(Collider2D obj)
    {
      if(obj.attachedRigidbody)
        AddRigidbody(obj.attachedRigidbody);
    }

    private void OnExitSlowdownZone(Collider2D obj)
    {
      if(obj.attachedRigidbody)
        RemoveRigidbody(obj.attachedRigidbody);
    }

    private void ApplyPhysMaterial(Collider2D obj)
    {
      _origPhysMaterials[obj] = obj.sharedMaterial;
      obj.sharedMaterial = Config.PhysMaterialToApply;
    }

    private void ResetPhysMaterial(Collider2D obj)
    {
      obj.sharedMaterial = _origPhysMaterials[obj];
      _origPhysMaterials.Remove(obj);
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
      if (!_collidedRbs.Remove(rb))
        return;
      
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
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -Config.MaxSpeed, Config.MaxSpeed);
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
