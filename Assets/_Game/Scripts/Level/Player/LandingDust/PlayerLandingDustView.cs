using NaughtyAttributes;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player.LandingDust
{
  public class PlayerLandingDustView : MonoBehaviour, IGameObjectView
  {
    [SerializeField, Required] private PlayerConfig _config;
    [SerializeField, Required] private Transform _vfxSpawnPoint;

    public void Play() => 
      Instantiate(_config.LandingDustVFXPrefab, _vfxSpawnPoint.position, Quaternion.identity);
  }
}
