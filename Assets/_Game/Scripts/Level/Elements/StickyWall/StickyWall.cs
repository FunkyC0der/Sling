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

    private const float EntryTolerance = 0.05f;
    private static readonly List<Collider2D> _attachedColliders = new();

    private Rigidbody2D _carrierRb;
    private Vector2 _lastCarrierPosition;
    private Collider2D _slowdownCollider;

    private void Awake()
    {
      _slowdownCollider = _slowdownTriggerZone.GetComponent<Collider2D>();

      _carrierRb = GetComponentInParent<Rigidbody2D>();
      if (_carrierRb != null)
        _lastCarrierPosition = _carrierRb.position;

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

      SnapToStickDepth(rb, otherCollider);
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

    /// <summary>
    /// The trigger fires after the physics step, so the body has already sunk into the zone by a
    /// speed- and step-phase-dependent amount. Correct it to a fixed <see cref="StickyWallConfig.StickDepth"/>.
    /// Only applied to horizontal hits; bodies entering through the zone's ends are left as is.
    /// </summary>
    private void SnapToStickDepth(Rigidbody2D rb, Collider2D otherCollider)
    {
      if (_launchImmunities.ContainsKey(rb))
        return;

      Bounds zone = _slowdownCollider.bounds;
      Bounds body = GetSolidBounds(rb, otherCollider);

      float carrierVelX = _carrierRb != null ? _carrierRb.linearVelocity.x : 0f;
      float relVelX = rb.linearVelocity.x - carrierVelX;
      bool fromRight = body.center.x > zone.center.x;

      // Must be moving into the wall from its side.
      if (fromRight ? relVelX >= 0f : relVelX <= 0f)
        return;

      float penetration = fromRight ? zone.max.x - body.min.x : body.max.x - zone.min.x;

      // Deeper than one step of travel → entered through the top/bottom, not a side hit.
      float maxStepTravel = Mathf.Abs(relVelX) * Time.fixedDeltaTime + EntryTolerance;
      if (penetration > maxStepTravel)
        return;

      float shift = penetration - Config.StickDepth;
      rb.position += new Vector2(fromRight ? shift : -shift, 0f);
      rb.linearVelocity = new Vector2(carrierVelX, rb.linearVelocity.y);
    }

    private static Bounds GetSolidBounds(Rigidbody2D rb, Collider2D fallback)
    {
      int count = rb.GetAttachedColliders(_attachedColliders);
      bool hasBounds = false;
      Bounds result = default;
      for (int i = 0; i < count; i++)
      {
        Collider2D collider = _attachedColliders[i];
        if (collider.isTrigger || !collider.enabled)
          continue;

        if (hasBounds)
          result.Encapsulate(collider.bounds);
        else
          result = collider.bounds;
        hasBounds = true;
      }

      _attachedColliders.Clear();
      return hasBounds ? result : fallback.bounds;
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

    private Vector2 GetCarrierVelocity()
    {
      if (_carrierRb == null)
        return Vector2.zero;

      Vector2 currentPosition = _carrierRb.position;
      Vector2 velocity = _carrierRb.linearVelocity;
      if (velocity.sqrMagnitude <= 0f && Time.fixedDeltaTime > 0f)
        velocity = (currentPosition - _lastCarrierPosition) / Time.fixedDeltaTime;

      _lastCarrierPosition = currentPosition;
      return velocity;
    }

    private void ApplySlowdown()
    {
      Vector2 carrierVel = GetCarrierVelocity();

      foreach (Rigidbody2D rb in _collidedRbs.Keys)
      {
        if (_launchImmunities.ContainsKey(rb))
          continue;

        Vector2 relative = rb.linearVelocity - carrierVel;
        relative.x = 0f;
        relative.y = Mathf.Clamp(relative.y, -Config.MaxSpeed, Config.MaxSpeed);
        rb.linearVelocity = carrierVel + relative;
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
