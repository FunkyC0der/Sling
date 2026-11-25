using Playtika.Controllers;
using Sling.Common.Extensions;
using UnityEngine;

namespace Sling.Settings
{
  // Loads the settings from PlayerPrefs and saves every change right away.
  public class SettingsController : ControllerBase
  {
    private const string kMusicVolumeKey = "Sling.Settings.MusicVolume";
    private const string kSfxVolumeKey = "Sling.Settings.SfxVolume";
    private const string kCursorVisibleKey = "Sling.Settings.CursorVisible";

    private readonly SettingsModel _model;

    public SettingsController(IControllerFactory controllerFactory, SettingsModel model)
      : base(controllerFactory)
    {
      _model = model;
    }

    protected override void OnStart()
    {
      _model.MusicVolume.Value = LoadVolume(kMusicVolumeKey);
      _model.SfxVolume.Value = LoadVolume(kSfxVolumeKey);
      _model.IsCursorVisible.Value = PlayerPrefs.GetInt(kCursorVisibleKey, 0) != 0;

      _model.MusicVolume.OnValueChanged += OnMusicVolumeChanged;
      _model.SfxVolume.OnValueChanged += OnSfxVolumeChanged;
      _model.IsCursorVisible.OnValueChanged += OnCursorVisibleChanged;

      this.AddDisposableAction(() =>
      {
        _model.MusicVolume.OnValueChanged -= OnMusicVolumeChanged;
        _model.SfxVolume.OnValueChanged -= OnSfxVolumeChanged;
        _model.IsCursorVisible.OnValueChanged -= OnCursorVisibleChanged;
      });
    }

    private static int LoadVolume(string key) =>
      Mathf.Clamp(PlayerPrefs.GetInt(key, SettingsModel.kMaxVolume), 0, SettingsModel.kMaxVolume);

    private static void Save(string key, int value)
    {
      PlayerPrefs.SetInt(key, value);
      PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(int oldValue, int newValue) =>
      Save(kMusicVolumeKey, newValue);

    private void OnSfxVolumeChanged(int oldValue, int newValue) =>
      Save(kSfxVolumeKey, newValue);

    private void OnCursorVisibleChanged(bool oldValue, bool newValue) =>
      Save(kCursorVisibleKey, newValue ? 1 : 0);
  }
}
