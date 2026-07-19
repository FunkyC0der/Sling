using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common;
using Sling.Level.Common;
using Sling.Level.Elements.StickyWall;
using UnityEngine;

namespace Sling.Level.Collision
{
  public class IsWallSlidingController : ControllerBase<Observable<bool>>
  {
    private readonly CollisionTriggerView _collisionTriggerView;
    private readonly IPositionView _positionView;
    private readonly IFaceDirectionView _faceDirectionView;
    
    private readonly HashSet<GameObject> _collidedObjects = new();

    protected IsWallSlidingController(IControllerFactory controllerFactory,
      CollisionTriggerView collisionTriggerView,
      IFaceDirectionView faceDirectionView, 
      IPositionView positionView) 
      : base(controllerFactory)
    {
      _collisionTriggerView = collisionTriggerView;
      _faceDirectionView = faceDirectionView;
      _positionView = positionView;
    }

    protected override void OnStart()
    {
      _collisionTriggerView.Events.OnTriggerEnter += OnTriggerEnter;
      _collisionTriggerView.Events.OnTriggerExit += OnTriggerExit;
    }

    protected override void OnStop()
    {
      _collisionTriggerView.Events.OnTriggerEnter -= OnTriggerEnter;
      _collisionTriggerView.Events.OnTriggerExit -= OnTriggerExit; 
    }

    private void OnTriggerEnter(Collider2D collider) =>
      AddCollidedObject(collider.gameObject);
    
    private void OnTriggerExit(Collider2D collider) =>
      RemoveCollidedObject(collider.gameObject);     

    private void AddCollidedObject(GameObject obj)
    {
      if (!obj.GetComponent<StickyWall>())
        return;
      
      _collidedObjects.Add(obj);
      Args.Value = true;
    }

    private void RemoveCollidedObject(GameObject obj)
    {
      if (!obj.GetComponent<StickyWall>())
        return;
      
      _collidedObjects.Remove(obj);
      Args.Value = _collidedObjects.Count > 0;
    }
  }
}