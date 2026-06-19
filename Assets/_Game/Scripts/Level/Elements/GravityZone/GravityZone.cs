using System;
using System.Collections.Generic;
using Sling.Common.Collission;
using Sling.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Sling.Level.Elements.GravityZone
{
  public class GravityZone : MonoBehaviour
  {
    [SerializeField] private GravityZoneConfig _config;
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private Vector2Int _triggerZoneSize;
    
    private readonly HashSet<Rigidbody2D> _rigidbodies = new();

    private void Awake()
    {
      _triggerZone.OnEnter += OnEnter;
      _triggerZone.OnExit += OnExit;
    }

    private void OnDestroy()
    {
      _triggerZone.OnEnter -= OnEnter;
      _triggerZone.OnExit -= OnExit;
    }

    private void OnEnter(Collider2D other)
    {
      if(other.attachedRigidbody)
      {
        if(_rigidbodies.Add(other.attachedRigidbody))
          other.attachedRigidbody.gravityScale *= _config.GravityScaleMultiplier;
      }
    }

    private void OnExit(Collider2D other)
    {
      if(other.attachedRigidbody)
      {
        if (_rigidbodies.Remove(other.attachedRigidbody)) 
          other.attachedRigidbody.gravityScale /= _config.GravityScaleMultiplier;
      }
    }

    private void FixedUpdate()
    {
      foreach (Rigidbody2D rb in _rigidbodies) 
        rb.AddForce(_config.Force * transform.up, ForceMode2D.Force);
    }
    
#if UNITY_EDITOR
    [SerializeField] private BoxCollider2D _triggerZoneCollider;
    [SerializeField] private Transform _spriteRoot;
    [SerializeField] private ParticleSystem _windVFX;

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;

      _triggerZoneCollider.size = new Vector2(_triggerZoneSize.x, _triggerZoneSize.y);
      _spriteRoot.localPosition = new Vector3(0, -_triggerZoneSize.y * 0.5f, 0);

      if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
        return;
      
      GravityZone prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(this);
      if (!prefabAsset)
        return;

      float ratioX = (float)_triggerZoneSize.x / prefabAsset._triggerZoneSize.x;
      float ratioY = (float)_triggerZoneSize.y / prefabAsset._triggerZoneSize.y;
      
      // Match sprite
      _spriteRoot.localScale = prefabAsset._spriteRoot.localScale * ratioX;
      
      MatchWindVFXProperties(ratioX, ratioY, prefabAsset._windVFX);
    }

    private void MatchWindVFXProperties(float ratioX, float ratioY, ParticleSystem prefabVFX)
    {
      _windVFX.transform.localPosition = prefabVFX.transform.localPosition.Multiply(ratioX, ratioY);

      // Update Shape
      ParticleSystem.ShapeModule shape = _windVFX.shape;
      shape.scale = prefabVFX.shape.scale.Multiply(ratioX, 1, ratioY);

      // Update Main Module
      ParticleSystem.MainModule main = _windVFX.main;
      
      ParticleSystem.MinMaxCurve newStartLifetime = prefabVFX.main.startLifetime;
      newStartLifetime.constant *= ratioY;
      
      main.startLifetime = newStartLifetime;

      ParticleSystem.MinMaxCurve newStartSpeed = prefabVFX.main.startSpeed;
      newStartSpeed.constantMin *= ratioY;
      newStartSpeed.constantMax *= ratioY;
      
      //main.startSpeed = newStartSpeed;
      
      // Update Velocity Over Lifetime
      ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _windVFX.velocityOverLifetime;

      ParticleSystem.MinMaxCurve newVelocityX = velocityOverLifetime.x;
      newVelocityX.curveMultiplier *= ratioX;

      // Update Emission
      ParticleSystem.EmissionModule emission = _windVFX.emission;

      ParticleSystem.MinMaxCurve newRateOverTime = prefabVFX.emission.rateOverTime;
      newRateOverTime.constant *= (ratioX + ratioY) * 0.5f;
      emission.rateOverTime = newRateOverTime;
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(transform.position, new Vector3(_triggerZoneSize.x, _triggerZoneSize.y, 0));
    }
#endif // UNITY_EDITOR
  }
}