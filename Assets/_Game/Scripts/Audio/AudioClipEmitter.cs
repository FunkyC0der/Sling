using UnityEngine;

namespace Sling.Audio
{
  public class AudioClipEmitter : MonoBehaviour
  {
    [SerializeField] private AudioClipId _clipId = AudioClipId.Invalid;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioConfig _audioConfig;

    public void PlayOneShot()
    {
      AudioClipConfig clipConfig = _audioConfig.GetClipConfig(_clipId);
      _audioSource.PlayOneShot(clipConfig.AudioClip, clipConfig.Volume);
    }
  }
}