using NaughtyAttributes;
using UnityEngine;

namespace Sling.Audio
{
  public class AudioClipEmitter : MonoBehaviour
  {
    [SerializeField] private AudioClipId _clipId = AudioClipId.Invalid;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioConfig _audioConfig;
    [MinMaxSlider(0.5f, 1.5f)]
    [SerializeField] private Vector2 _pitchRange = Vector2.one;

    public void Play()
    {
      AudioClipConfig clipConfig = _audioConfig.GetClipConfig(_clipId);
      _audioSource.clip = clipConfig.AudioClip;
      _audioSource.volume = clipConfig.Volume;
      _audioSource.pitch = RandomPitch();
      _audioSource.Play();
    }

    public void PlayOneShot()
    {
      AudioClipConfig clipConfig = _audioConfig.GetClipConfig(_clipId);
      _audioSource.pitch = RandomPitch();
      _audioSource.PlayOneShot(clipConfig.AudioClip, clipConfig.Volume);
    }

    private float RandomPitch() =>
      Random.Range(_pitchRange.x, _pitchRange.y);
  }
}