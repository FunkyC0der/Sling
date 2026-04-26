using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class HazardView : MonoBehaviour
{
    public event Action OnPlayerHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            OnPlayerHit?.Invoke();
    }
}
