using UnityEngine;

namespace Sling.Level.Player
{
  [ExecuteAlways]
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerAutoTweaker : MonoBehaviour
  {
#if UNITY_EDITOR
    [SerializeField] private PlayerConfig _config;

    private void FixedUpdate()
    {
      if (!_config)
        return;

      Physics2D.gravity = new Vector2(Physics2D.gravity.x, _config.GlobalGravity);
    }
#endif
  }
}