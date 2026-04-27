using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class HazardView : BaseView
{
    public event Action OnPlayerHit;

    private void OnTriggerEnter2D(Collider2D other) =>
        HitDetector.RaiseOnPlayerCollision(other, OnPlayerHit);
}
