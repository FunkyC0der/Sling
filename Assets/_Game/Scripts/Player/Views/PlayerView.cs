using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Player.Views
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerView : BaseView
  {
    public event Action OnFixedTick;
    
    [field: SerializeField] public PlayerConfig Config { get; private set; }

    private Rigidbody2D _rb;

    public float Mass => _rb.mass;
    public float VelocityY => _rb.linearVelocityY;

    private void Awake() =>
      _rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate() =>
      OnFixedTick?.Invoke();

    public void Launch(Vector2 force) =>
      _rb.AddForce(force, ForceMode2D.Impulse);

    public void SetVelocityY(float y) =>
      _rb.linearVelocityY = y;
  }
}
