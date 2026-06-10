using System;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Hazards
{
  public class DamageableView : MonoBehaviour, IDamageable, IGameObjectView
  {
    public event Action OnDamaged;
    
    public void TakeDamage() => 
      OnDamaged?.Invoke();
  }
}