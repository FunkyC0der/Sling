using System;
using UnityEngine;

namespace Sling.Level.Boss
{
  [RequireComponent(typeof(Collider2D))]
  public class WeakPointView : MonoBehaviour
  {
    public event Action OnHit;

    private void OnCollisionEnter2D() => 
      OnHit?.Invoke();

    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);
  }
}
