using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    public class MovingSawView : BaseView
    {
        [field: SerializeField] public Transform PointA { get; private set; }
        [field: SerializeField] public Transform PointB { get; private set; }

        public Vector3 Position => transform.position;

        public event Action OnPlayerHit;

        public void SetPosition(Vector3 pos) => transform.position = pos;

        private void OnTriggerEnter2D(Collider2D other) =>
            HitDetector.RaiseOnPlayerCollision(other, OnPlayerHit);
    }
}
