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

    private readonly Dictionary<Rigidbody2D, HashSet<Collider2D>> _collidedRbs = new();
    private readonly Dictionary<ILaunchable, Action> _launchSubscriptions = new();
    private readonly Dictionary<Collider2D, PhysicsMaterial2D> _origPhysMaterials = new();
    private readonly Dictionary<Rigidbody2D, LaunchImmunity> _launchImmunities = new();

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
        AddRigidbody(obj.attachedRigidbody, obj);
    }

    private void OnExitSlowdownZone(Collider2D obj)
    {
      if(obj.attachedRigidbody)
        RemoveRigidbody(obj.attachedRigidbody, obj);
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

    private void AddRigidbody(Rigidbody2D rb, Collider2D otherCollider)
    {
      if (_collidedRbs.TryGetValue(rb, out HashSet<Collider2D> colliders))
      {
        colliders.Add(otherCollider);
        return;
      }

      _collidedRbs.Add(rb, new HashSet<Collider2D> { otherCollider });

      _stickClipEmitter.PlayOneShot();
      
      var launchable = rb.GetComponent<ILaunchable>();
      if (launchable != null)
      {
        Action onLaunched = () =>
          ResetLaunchImmunity(rb);

        _launchSubscriptions.Add(launchable, onLaunched);
        launchable.OnLaunched += onLaunched;
      }
    }

    private void RemoveRigidbody(Rigidbody2D rb, Collider2D otherCollider)
    {
      if (!_collidedRbs.TryGetValue(rb, out HashSet<Collider2D> colliders))
        return;

      colliders.Remove(otherCollider);
      if (colliders.Count > 0)
        return;

      RemoveRigidbody(rb);
    }

    private void RemoveRigidbody(Rigidbody2D rb)
    {
      if (!_collidedRbs.Remove(rb))
        return;

      _launchImmunities.Remove(rb);
      
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
      UpdateLaunchImmunity();
      ApplySlowdown();
    }

    private void UpdateLaunchImmunity()
    {
      foreach (LaunchImmunity immunity in _launchImmunities.Values)
        immunity.RemainingTime -= Time.fixedDeltaTime;

      while (TryGetExpiredLaunchImmunity(out Rigidbody2D rb))
        _launchImmunities.Remove(rb);
    }

    private void ResetLaunchImmunity(Rigidbody2D rb)
    {
      if (!_launchImmunities.TryGetValue(rb, out LaunchImmunity immunity))
      {
        immunity = new LaunchImmunity();
        _launchImmunities.Add(rb, immunity);
      }

      immunity.RemainingTime = Config.LaunchImmunityDuration;
    }

    private bool TryGetExpiredLaunchImmunity(out Rigidbody2D rb)
    {
      foreach (KeyValuePair<Rigidbody2D, LaunchImmunity> entry in _launchImmunities)
      {
        if (entry.Value.RemainingTime >= 0f)
          continue;

        rb = entry.Key;
        return true;
      }

      rb = null;
      return false;
    }

    private void ApplySlowdown()
    {
      foreach (Rigidbody2D rb in _collidedRbs.Keys)
      {
        if (_launchImmunities.ContainsKey(rb))
          continue;

        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -Config.MaxSpeed, Config.MaxSpeed);
      }
    }

    private void OnDisable()
    {
      foreach (KeyValuePair<ILaunchable, Action> subscription in _launchSubscriptions)
        subscription.Key.OnLaunched -= subscription.Value;

      _launchSubscriptions.Clear();
      _collidedRbs.Clear();
      _launchImmunities.Clear();
    }

    private sealed class LaunchImmunity
    {
      public float RemainingTime { get; set; }
    }
  }
}
