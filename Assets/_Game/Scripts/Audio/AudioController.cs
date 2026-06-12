using System.Collections.Generic;
using Playtika.Controllers;

namespace Sling.Audio
{
  public class AudioController : ControllerBase
  {
    private readonly AudioView _view;
    private readonly AudioConfig _config;
    private readonly AudioEvents _audioEvents;

    private AudioClipId _currentMusicClipId = AudioClipId.Invalid;

    public AudioController(IControllerFactory controllerFactory,
      AudioView view,
      AudioConfig config,
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _view = view;
      _config = config;
      _audioEvents = audioEvents;
    }

    protected override void OnStart()
    {
      _audioEvents.PlayMusic += PlayMusic;
      _audioEvents.PlaySFX += PlaySFX;
      _audioEvents.PlayConcurrentSFX += PlayConcurrentSFX;
    }

    private void PlayMusic(AudioClipId id)
    {
      if (_currentMusicClipId == id)
        return;

      _currentMusicClipId = id;
      _view.PlayMusic(_config.AudioClips.GetValueOrDefault(id));
    }

    private void PlaySFX(AudioClipId id) =>
      _view.PlaySFX(_config.AudioClips.GetValueOrDefault(id));

    private void PlayConcurrentSFX(AudioClipId id) => 
      _view.PlayConcurrentSFX(_config.AudioClips.GetValueOrDefault(id));
  }
}