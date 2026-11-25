using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common;
using Sling.Common.UI;
using Sling.Infrastructure.UI;
using Sling.Settings;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Sling.Level.Hud
{
  public class PauseWindowController : ControllerWithResultBase<PauseWindowResult>
  {
    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;
    private readonly HudView _hudView;
    private readonly SettingsModel _settingsModel;

    private VisualElement _window;
    private bool _isSettingsOpen;

    public PauseWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig,
      HudView hudView,
      SettingsModel settingsModel)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
      _hudView = hudView;
      _settingsModel = settingsModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      CursorLockMode previousCursorLockState = Cursor.lockState;

      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;

      PauseWindowResult result;
      try
      {
        _window = _popUpWindowsRootView.CreateWindow(_gameConfig.PauseWindowUxml, hasBackground: true);

        try
        {
          _hudView.SetBlurred(true);
          await _popUpWindowsRootView.ShowWindow(_window, ct);
          result = await WaitForResult(ct);
          _hudView.SetBlurred(false);
          await _popUpWindowsRootView.HideWindow(_window, ct);
        }
        finally
        {
          _hudView.SetBlurred(false);
          _popUpWindowsRootView.RemoveWindow(_window);
        }
      }
      finally
      {
        // The setting may have been changed in the settings window opened from the pause.
        Cursor.visible = _settingsModel.IsCursorVisible.Value;
        Cursor.lockState = previousCursorLockState;
      }

      Complete(result);
    }

    private async UniTask<PauseWindowResult> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<PauseWindowResult>();

      _window.Q<Button>(WindowNames.RestartButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Restart);

      _window.Q<Button>(WindowNames.SettingsButton).clicked += () =>
        OpenSettingsAsync(ct).Forget();

      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Menu);

      // Esc (the pause key) is handled by the pop-up root: it closes only the top pop-up.
      (bool buttonClicked, PauseWindowResult result) =
        await UniTaskUtils.WhenAnyWithAutoCancel(
          token => completionSource.Task.AttachExternalCancellation(token),
          token => _popUpWindowsRootView.GetDismissWaitTask(_window, token),
          ct);

      return buttonClicked ? result : PauseWindowResult.Resume;
    }

    private async UniTaskVoid OpenSettingsAsync(CancellationToken ct)
    {
      if (_isSettingsOpen)
        return;

      _isSettingsOpen = true;
      try
      {
        await ExecuteAndWaitResultAsync<SettingsWindowController>(ct);
      }
      finally
      {
        _isSettingsOpen = false;
      }
    }
  }
}
