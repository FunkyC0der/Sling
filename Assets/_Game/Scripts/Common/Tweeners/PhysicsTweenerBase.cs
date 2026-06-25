using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  [RequireComponent(typeof(Rigidbody2D))]
  public abstract class PhysicsTweenerBase : MonoBehaviour
  {
    [SerializeField] private bool _autoStart = true;

    protected Rigidbody2D _rigidbody;
    protected Sequence _sequence;
    
    public Rigidbody2D Rigidbody => _rigidbody;
    public bool IsActive => _sequence.isAlive;

    protected virtual void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    protected virtual void Start()
    {
      if (_autoStart)
        StartTween();
    }

    protected virtual void OnDestroy() => StopTween();

    public abstract void StartTween();
    public void StopTween() => _sequence.Stop();
  }
}
