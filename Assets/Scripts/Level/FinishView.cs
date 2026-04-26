using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinishView : MonoBehaviour
{
    public event Action OnPlayerReachedFinish;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            OnPlayerReachedFinish?.Invoke();
    }
}
