using System;

namespace Sling.Audio
{
  public class AudioEvents
  {
    public Action<AudioClipId> PlayMusic;
    public Action<AudioClipId> PlaySFX;
    public Action<AudioClipId> PlayConcurrentSFX;
  }
}