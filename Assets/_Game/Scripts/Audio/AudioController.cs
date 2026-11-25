using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Settings;

namespace Sling.Audio
{
  public class AudioController : ControllerBase
  {
    private readonly AudioView _view;
    private readonly AudioConfig _config;
    private readonly AudioEvents _audioEvents;
    private readonly SettingsModel _settingsModel;

    private AudioClipId _currentMusicClipId = AudioClipId.Invalid;

    public AudioController(IControllerFactory controllerFactory,
      AudioView view,
      AudioConfig config,
      AudioEvents audioEvents,
      SettingsModel settingsModel)
      : base(controllerFactory)
    {
      _view = view;
      _config = config;
      _audioEvents = audioEvents;
      _settingsModel = settingsModel;
    }

    protected override void OnStart()
    {
      _audioEvents.PlayMusic += PlayMusic;
      _audioEvents.PlaySFX += PlaySFX;
      _audioEvents.PlayConcurrentSFX += PlayConcurrentSFX;

      _view.SetMusicVolumeMultiplier(SettingsModel.ToVolumeMultiplier(_settingsModel.MusicVolume.Value));
      _view.SetSfxVolumeMultiplier(SettingsModel.ToVolumeMultiplier(_settingsModel.SfxVolume.Value));

      _settingsModel.MusicVolume.OnValueChanged += OnMusicVolumeChanged;
      _settingsModel.SfxVolume.OnValueChanged += OnSfxVolumeChanged;
      this.AddDisposableAction(() =>
      {
        _settingsModel.MusicVolume.OnValueChanged -= OnMusicVolumeChanged;
        _settingsModel.SfxVolume.OnValueChanged -= OnSfxVolumeChanged;
      });
    }

    private void OnMusicVolumeChanged(int oldValue, int newValue) =>
      _view.SetMusicVolumeMultiplier(SettingsModel.ToVolumeMultiplier(newValue));

    private void OnSfxVolumeChanged(int oldValue, int newValue) =>
      _view.SetSfxVolumeMultiplier(SettingsModel.ToVolumeMultiplier(newValue));

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
