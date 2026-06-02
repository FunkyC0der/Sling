using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common;
using Sling.Common.Extensions;
using UnityEngine;

namespace Sling.Level.Collision
{
  public class IsInAirController : ControllerBase<IsInAirController.Context>
  {
    public class Context
    {
      public readonly Observable<bool> IsInAir;
      public readonly LayerMask GroundSurfaceLayerMask;

      public Context(Observable<bool> isInAir, LayerMask groundSurfaceLayerMask)
      {
        IsInAir = isInAir;
        GroundSurfaceLayerMask = groundSurfaceLayerMask;
      }
    }

    private readonly CollisionTriggerView _collisionTriggerView;
    private readonly HashSet<GameObject> _collidedObjects = new();

    protected IsInAirController(IControllerFactory controllerFactory, CollisionTriggerView collisionTriggerView) 
      : base(controllerFactory)
    {
      _collisionTriggerView = collisionTriggerView;
    }

    protected override void OnStart()
    {
      _collisionTriggerView.Events.OnEnter += OnCollisionEnter;
      _collisionTriggerView.Events.OnExit += OnCollisionExit;
      _collisionTriggerView.Events.OnTriggerEnter += OnTriggerEnter;
      _collisionTriggerView.Events.OnTriggerExit += OnTriggerExit;
    }

    protected override void OnStop()
    {
      _collisionTriggerView.Events.OnEnter -= OnCollisionEnter;
      _collisionTriggerView.Events.OnExit -= OnCollisionExit;
      _collisionTriggerView.Events.OnTriggerEnter -= OnTriggerEnter;
      _collisionTriggerView.Events.OnTriggerExit -= OnTriggerExit; 
    }

    private void OnCollisionEnter(Collision2D collision) => 
      AddCollidedObject(collision.gameObject);
    
    private void OnCollisionExit(Collision2D collision) => 
      RemoveCollidedObject(collision.gameObject);
    
    private void OnTriggerEnter(Collider2D collider) =>
      AddCollidedObject(collider.gameObject);
    
    private void OnTriggerExit(Collider2D collider) =>
      RemoveCollidedObject(collider.gameObject);     

    private void AddCollidedObject(GameObject obj)
    {
      if (!Args.GroundSurfaceLayerMask.HasLayer(obj.layer))
        return;
      
      _collidedObjects.Add(obj);
      Args.IsInAir.Value = false;
    }

    private void RemoveCollidedObject(GameObject obj)
    {
      if (!Args.GroundSurfaceLayerMask.HasLayer(obj.layer))
        return;
      
      _collidedObjects.Remove(obj);
      Args.IsInAir.Value = _collidedObjects.Count == 0;
    }
  }
}