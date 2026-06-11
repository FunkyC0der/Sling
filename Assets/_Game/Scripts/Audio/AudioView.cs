using Sling.Common.Views;
using UnityEngine;

namespace Sling.Audio
{
  public class AudioView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    public void PlayMusic(AudioClipConfig audioClipConfig)
    {
      _musicAudioSource.clip = audioClipConfig.AudioClip;
      _musicAudioSource.volume = audioClipConfig.Volume;
      _musicAudioSource.loop = true;
      _musicAudioSource.Play();
    }

    public void PlaySFX(AudioClipConfig audioClipConfig) =>
      _sfxAudioSource.PlayOneShot(audioClipConfig.AudioClip, audioClipConfig.Volume);

    public void PlayConcurrentSFX(AudioClipConfig audioClipConfig)
    {
      _sfxAudioSource.clip = audioClipConfig.AudioClip;
      _sfxAudioSource.volume = audioClipConfig.Volume;
      _sfxAudioSource.loop = false;
      _sfxAudioSource.Play();
    }
  }
}