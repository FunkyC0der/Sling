using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common;
using Sling.Common.UI;
using Sling.Infrastructure.UI;
using UnityEngine.UIElements;

namespace Sling.Level.Hud
{
  public class PauseWindowController : ControllerWithResultBase<PauseWindowResult>
  {
    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;

    private VisualElement _window;

    public PauseWindowController(
      IControllerFactory factory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig)
      : base(factory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.PauseWindowUxml, hasBackground: true);

      PauseWindowResult result;
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

      Complete(result);
    }

    private async UniTask<PauseWindowResult> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<PauseWindowResult>();

      _window.Q<Button>(WindowNames.RestartButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Restart);

      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Menu);

      (bool buttonClicked, PauseWindowResult result) =
        await UniTaskUtils.WhenAnyWithAutoCancel(
          token => completionSource.Task.AttachExternalCancellation(token),
          token => _popUpWindowsRootView.GetClickOnBackgroundWaitTask(_window, token),
          ct);

      return buttonClicked ? result : PauseWindowResult.Resume;
    }
  }
}
