using Cysharp.Threading.Tasks;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerView : MonoBehaviour, IUniqueView
  {
    [field: SerializeField] public PlayerConfig Config { get; private set; }
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private Rigidbody2D _rb;

    public float LinearVelocityX
    {
      get => _rb.linearVelocityX;
      set => _rb.linearVelocityX = value;
    }

    public Vector3 Position => _rb.position;

    private void Awake() =>
      _rb = GetComponent<Rigidbody2D>();

    public void SetGravityScale(float gravityScale) =>
      _rb.gravityScale = gravityScale;

    public async UniTask Die()
    {
      _rb.bodyType = RigidbodyType2D.Static;

      Color originalColor = _spriteRenderer.color;
      float flickerPeriod = Config.DieDuration / Config.DieFlickerCount;

      for (int i = 0; i < Config.DieFlickerCount; ++i)
      {
        _spriteRenderer.color = Color.red;
        await UniTask.WaitForSeconds(flickerPeriod * 0.5f);
        _spriteRenderer.color = originalColor;
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
