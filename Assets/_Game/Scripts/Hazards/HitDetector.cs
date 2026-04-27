using System;
using UnityEngine;

namespace Sling.Hazards
{
    internal static class HitDetector
    {
        public static void RaiseOnPlayerCollision(Collider2D other, Action handler)
        {
            if (other.CompareTag("Player"))
                handler?.Invoke();
        }
    }
}
