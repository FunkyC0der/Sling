using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class FinishView : BaseView
    {
        public event Action OnReached;

        private void OnTriggerEnter2D(Collider2D other) => 
            OnReached?.Invoke();
    }
}
