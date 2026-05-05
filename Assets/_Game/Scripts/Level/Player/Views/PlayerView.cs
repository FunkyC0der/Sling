using System;
using Cysharp.Threading.Tasks;
using Sling.Core;
using UnityEngine;

namespace Sling.Level.Player.Views
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

    public async UniTask Die()
    {
      _rb.bodyType = RigidbodyType2D.Static;

      var sprite = GetComponent<SpriteRenderer>();
      Color originalColor = sprite.color;
      float flickerPeriod = Config.DieDuration / Config.DieFlickerCount;

      for (int i = 0; i < Config.DieFlickerCount; ++i)
      {
        sprite.color = Color.white;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f);
        sprite.color = originalColor;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f);
      }
      
      gameObject.SetActive(false); // Hide the player from view
    }
  }
}
