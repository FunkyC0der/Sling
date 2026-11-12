using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common;
using Sling.Common.UI;
using Sling.Infrastructure.UI;
using Sling.Level.Player;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Sling.Level.Hud
{
  public class PauseWindowController : ControllerWithResultBase<PauseWindowResult>
  {
    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;
    private readonly PlayerInputView _inputView;

    private VisualElement _window;

    public PauseWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig,
      PlayerInputView inputView)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
      _inputView = inputView;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      bool wasCursorVisible = Cursor.visible;
      CursorLockMode previousCursorLockState = Cursor.lockState;

      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;

      PauseWindowResult result;
      try
      {
        _window = _popUpWindowsRootView.CreateWindow(_gameConfig.PauseWindowUxml, hasBackground: true);

        try
        {
          await _popUpWindowsRootView.ShowWindow(_window, ct);
          result = await WaitForResult(ct);
          await _popUpWindowsRootView.HideWindow(_window, ct);
        }
        finally
        {
          _popUpWindowsRootView.RemoveWindow(_window);
        }
      }
      finally
      {
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = previousCursorLockState;
      }

      Complete(result);
    }

    private async UniTask<PauseWindowResult> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<PauseWindowResult>();

      _window.Q<Button>(WindowNames.RestartButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Restart);

      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Menu);

      // The same key that opened the pause closes it.
      void OnResumeRequested() => completionSource.TrySetResult(PauseWindowResult.Resume);
      _inputView.OnPauseRequested += OnResumeRequested;

      try
      {
        (bool buttonClicked, PauseWindowResult result) =
          await UniTaskUtils.WhenAnyWithAutoCancel(
            token => completionSource.Task.AttachExternalCancellation(token),
            token => _popUpWindowsRootView.GetClickOnBackgroundWaitTask(_window, token),
            ct);

        return buttonClicked ? result : PauseWindowResult.Resume;
      }
      finally
      {
        _inputView.OnPauseRequested -= OnResumeRequested;
      }
    }
  }
}
