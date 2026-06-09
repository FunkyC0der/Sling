using NaughtyAttributes;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerView : MonoBehaviour, IUniqueView
  {
    [field: SerializeField] public PlayerConfig Config { get; private set; }
    [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }
    [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }

    [Button]
    public void FlipBodySpriteX() => 
      BodySprite.flipX = !BodySprite.flipX;
  }
}
