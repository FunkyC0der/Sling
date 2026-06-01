using System;
using System.Collections.Generic;
using Sling.Common.Extensions;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerIsInAirView : MonoBehaviour, IUniqueView
  {
    public Action OnStateChanged;
    
    [SerializeField] private LayerMask _surfaceLayerMask;
    
    public bool IsInAir { get; private set; }
    
    private readonly HashSet<GameObject> _touchedObjects = new();    

    private void OnCollisionEnter2D(Collision2D other) => 
      OnTouched(other.gameObject);

    private void OnCollisionExit2D(Collision2D other) => 
      OnUntouched(other.gameObject);

    private void OnTriggerEnter2D(Collider2D other) => 
      OnTouched(other.gameObject);

    private void OnTriggerExit2D(Collider2D other) => 
      OnUntouched(other.gameObject);

    private void OnTouched(GameObject other)
    {
      if (!_surfaceLayerMask.HasLayer(other.layer))
        return;
      
      _touchedObjects.Add(other);

      if (IsInAir)
      {
        IsInAir = false;
        OnStateChanged?.Invoke();
      }
    }

    private void OnUntouched(GameObject other)
    {
      if (!_surfaceLayerMask.HasLayer(other.layer))
        return;
      
      _touchedObjects.Remove(other);

      if (_touchedObjects.Count == 0 && !IsInAir)
      {
        IsInAir = true;
        OnStateChanged?.Invoke();        
      }
    }
  }
}