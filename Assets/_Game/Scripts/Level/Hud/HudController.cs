using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Hud
{
  public class HudController : ControllerBase
  {
    private readonly HudView _hudView;
    private readonly PopupWindowsRootView _popupRootView;
    private readonly PlayerInputView _playerInput;
    private readonly LevelEvents _events;
    private readonly GameModel _gameModel;

    public HudController(
      IControllerFactory factory,
      HudView hudView,
      PopupWindowsRootView popupRootView,
      PlayerInputView playerInput,
      LevelEvents events, 
      GameModel gameModel)
      : base(factory)
    {
      _hudView = hudView;
      _popupRootView = popupRootView;
      _playerInput = playerInput;
      _events = events;
      _gameModel = gameModel;
    }

    protected override void OnStart()
    {
      _hudView.OnPauseClicked += HandlePauseClicked;
      _hudView.SetLevelIndex(_gameModel.LevelIndex);
    }

    protected override void OnStop() =>
      _hudView.OnPauseClicked -= HandlePauseClicked;

    private void HandlePauseClicked() =>
      ShowPauseAsync().Forget();

    private async UniTaskVoid ShowPauseAsync()
    {
      Time.timeScale = 0f;
      _playerInput.DisableInput();

      PauseWindowResult result;
      try
      {
        result = await ExecuteAndWaitResultAsync<PauseWindowController, IWindowRootView, PauseWindowResult>(
          _popupRootView, CancellationToken);
      }
      finally
      {
        Time.timeScale = 1f;
        _playerInput.EnableInput();
      }

      switch (result)
      {
        case PauseWindowResult.Restart:
          _events.OnRestartRequested?.Invoke();
          break;
        case PauseWindowResult.Menu:
          _events.OnMenuRequested?.Invoke();
          break;
      }
    }
  }
}
