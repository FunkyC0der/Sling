using System;
using Cysharp.Threading.Tasks;
using Sling.Common.Views;
using Sling.Level.Common;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerView : MonoBehaviour, IUniqueView, ILaunchable
  {
    public event Action OnLaunched;
    
    [field: SerializeField] public PlayerConfig Config { get; private set; }

    private Rigidbody2D _rb;

    public float Mass => _rb.mass;

    public float LinearVelocityX
    {
      get => _rb.linearVelocityX;
      set => _rb.linearVelocityX = value;
    }

    public Vector3 Position => _rb.position;

    private void Awake() =>
      _rb = GetComponent<Rigidbody2D>();

    public void Launch(Vector2 force)
    {
      _rb.linearVelocity = force;
      OnLaunched?.Invoke();
    }

    public void SetPhysicsEnabled(bool isEnabled) =>
      _rb.bodyType = isEnabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

    public async UniTask Die()
    {
      _rb.bodyType = RigidbodyType2D.Static;

      var sprite = GetComponent<SpriteRenderer>();
      Color originalColor = sprite.color;
      float flickerPeriod = Config.DieDuration / Config.DieFlickerCount;

      for (int i = 0; i < Config.DieFlickerCount; ++i)
      {
        sprite.color = Color.red;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f);
        sprite.color = originalColor;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f);
      }

      gameObject.SetActive(false); // Hide the player from view
    }

    public void Respawn(Vector3 position)
    {
      transform.position = position;
      _rb.bodyType = RigidbodyType2D.Dynamic;
      gameObject.SetActive(true);
    }
  }
}
