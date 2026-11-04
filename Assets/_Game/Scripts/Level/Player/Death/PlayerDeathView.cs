using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using Sling.Common.Tweeners;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player.Death
{
  /// <summary>Lives on the root Player object: Die() deactivates it, Revive() brings it back.</summary>
  public class PlayerDeathView : MonoBehaviour, IGameObjectView
  {
    [SerializeField, Required] private PlayerConfig _config;
    [SerializeField, Required] private SpriteBlinkTweener _blinkTweener;

    public async UniTask Die(CancellationToken cancellationToken)
    {
      gameObject.SetActive(false);
      Instantiate(_config.DeathVFXPrefab, transform.position, Quaternion.identity);

      _ = Tween.ShakeCamera(Camera.main, _config.DieCameraShakeStrength, frequency: _config.DieCameraShakeFrequency);

      await UniTask.WaitForSeconds(_config.DieDuration, cancellationToken: cancellationToken);
    }

    public async UniTask Revive()
    {
      gameObject.SetActive(true);
      await _blinkTweener.PlayBlink(_config.ReviveBlinkCount, _config.ReviveDuration, _config.ReviveBlinkStrength);
    }
  }
}
