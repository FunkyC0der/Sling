using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerView : MonoBehaviour, IUniqueView
  {
    [field: SerializeField] public PlayerConfig Config { get; private set; }

    [SerializeField, Required] private PlayerAnimationsView _animationsView;
    [SerializeField, Required] private SpriteRenderer _bodySprite;
    [SerializeField, Required] private Rigidbody2D _rigidbody;

    public Vector3 Position => transform.position;
    public bool IsFacingLeft => _bodySprite.flipX;
    public float LinearVelocityX => _rigidbody.linearVelocityX;
    public float LinearVelocityY => _rigidbody.linearVelocityY;

    public void SetPosition(Vector3 position) =>
      transform.position = position;

    public void SetFacingLeft(bool value) =>
      _bodySprite.flipX = value;

    public void FreezePhysics() =>
      _rigidbody.bodyType = RigidbodyType2D.Static;

    public void UnfreezePhysics() =>
      _rigidbody.bodyType = RigidbodyType2D.Dynamic;

    public void StopHorizontalMovement() =>
      _rigidbody.linearVelocityX = 0;

    public async UniTask StopHorizontalMovementAsync(float duration, CancellationToken cancellationToken)
    {
      if (duration <= 0 || Mathf.Approximately(_rigidbody.linearVelocityX, 0))
      {
        StopHorizontalMovement();
        return;
      }

      float deceleration = Mathf.Abs(_rigidbody.linearVelocityX) / duration;

      while (!Mathf.Approximately(_rigidbody.linearVelocityX, 0))
      {
        _rigidbody.linearVelocityX =
          Mathf.MoveTowards(_rigidbody.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);

        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }

      StopHorizontalMovement();
    }

    public void Show() => 
      gameObject.SetActive(true);

    [Button]
    private void FlipBodyFacing() => 
      SetFacingLeft(!IsFacingLeft);
  }
}
