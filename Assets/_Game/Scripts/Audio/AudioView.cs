using Sling.Common.Views;
using UnityEngine;

namespace Sling.Audio
{
  public class AudioView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    private float _musicClipVolume = 1f;
    private float _musicVolumeMultiplier = 1f;
    private float _sfxVolumeMultiplier = 1f;

    public void SetMusicVolumeMultiplier(float multiplier)
    {
      _musicVolumeMultiplier = multiplier;
      _musicAudioSource.volume = _musicClipVolume * _musicVolumeMultiplier;
    }

    public void SetSfxVolumeMultiplier(float multiplier) =>
      _sfxVolumeMultiplier = multiplier;

    public void PlayMusic(AudioClipConfig audioClipConfig)
    {
      _musicClipVolume = audioClipConfig.Volume;
      _musicAudioSource.clip = audioClipConfig.AudioClip;
      _musicAudioSource.volume = _musicClipVolume * _musicVolumeMultiplier;
      _musicAudioSource.loop = true;
      _musicAudioSource.Play();
    }

    public void PlaySFX(AudioClipConfig audioClipConfig) =>
      _sfxAudioSource.PlayOneShot(audioClipConfig.AudioClip, audioClipConfig.Volume * _sfxVolumeMultiplier);

    public void PlayConcurrentSFX(AudioClipConfig audioClipConfig)
    {
      _sfxAudioSource.clip = audioClipConfig.AudioClip;
      _sfxAudioSource.volume = audioClipConfig.Volume * _sfxVolumeMultiplier;
      _sfxAudioSource.loop = false;
      _sfxAudioSource.Play();
    }
  }
}
