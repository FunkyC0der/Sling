using System;
using UnityEngine;

namespace Sling.Level.Boss
{
  [RequireComponent(typeof(Collider2D))]
  public class WeakPointView : MonoBehaviour
  {
    public event Action OnHit;

    [SerializeField] private ParticleSystem _hideVFXPrefab;

    private void OnCollisionEnter2D() => 
      OnHit?.Invoke();

    public void Show() => gameObject.SetActive(true);

    public void Hide(bool showVFX)
    {
      gameObject.SetActive(false);

      if(showVFX)
        Instantiate(_hideVFXPrefab, transform.position, Quaternion.identity);
    }
  }
}
