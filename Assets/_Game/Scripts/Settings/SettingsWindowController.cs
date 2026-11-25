using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common;
using Sling.Common.UI;
using Sling.Infrastructure.UI;
using UnityEngine.UIElements;

namespace Sling.Settings
{
  public class SettingsWindowController : ControllerWithResultBase
  {
    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;
    private readonly SettingsModel _settingsModel;

    private VisualElement _window;

    public SettingsWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig,
      SettingsModel settingsModel)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
      _settingsModel = settingsModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.SettingsWindowUxml, hasBackground: true);

      try
      {
        InitWindow();
        await _popUpWindowsRootView.ShowWindow(_window, ct);
        await WaitForClose(ct);
        await _popUpWindowsRootView.HideWindow(_window, ct);
      }
      finally
      {
        _popUpWindowsRootView.RemoveWindow(_window);
      }

      Complete();
    }

    private void InitWindow()
    {
      BindVolumeSlider(_window.Q<SliderInt>(WindowNames.MusicVolumeSlider), _settingsModel.MusicVolume);
      BindVolumeSlider(_window.Q<SliderInt>(WindowNames.SfxVolumeSlider), _settingsModel.SfxVolume);

      var showCursorToggle = _window.Q<Toggle>(WindowNames.ShowCursorToggle);
      showCursorToggle.SetValueWithoutNotify(_settingsModel.IsCursorVisible.Value);
      showCursorToggle.RegisterValueChangedCallback(evt => _settingsModel.IsCursorVisible.Value = evt.newValue);
    }

    private static void BindVolumeSlider(SliderInt slider, Observable<int> volume)
    {
      slider.lowValue = 0;
      slider.highValue = SettingsModel.kMaxVolume;
      slider.SetValueWithoutNotify(volume.Value);
      slider.RegisterValueChangedCallback(evt => volume.Value = evt.newValue);

      // Highlight the fill while the knob is dragged, even when the pointer leaves the slider.
      slider.RegisterCallback<PointerCaptureEvent>(
        _ => slider.AddToClassList(WindowNames.Classes.SettingsSliderActive), TrickleDown.TrickleDown);
      slider.RegisterCallback<PointerCaptureOutEvent>(
        _ => slider.RemoveFromClassList(WindowNames.Classes.SettingsSliderActive), TrickleDown.TrickleDown);
    }

    private async UniTask WaitForClose(CancellationToken ct)
    {
      var backClickSource = new UniTaskCompletionSource<bool>();
      _window.Q<Button>(WindowNames.BackButton).clicked += () => backClickSource.TrySetResult(true);

      await UniTaskUtils.WhenAnyWithAutoCancel(
        token => backClickSource.Task.AttachExternalCancellation(token),
        token => _popUpWindowsRootView.GetDismissWaitTask(_window, token),
        ct);
    }
  }
}
