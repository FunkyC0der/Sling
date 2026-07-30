using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Hud;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerPauseController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly HudView _hudView;
    private readonly LevelEvents _events;

    private bool _isPauseOpen;

    public PlayerPauseController(
      IControllerFactory controllerFactory,
      PlayerInputView inputView,
      HudView hudView,
      LevelEvents events)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _hudView = hudView;
      _events = events;
    }

    protected override void OnStart()
    {
      _inputView.OnPauseRequested += OnPauseRequested;
      this.AddDisposableAction(() => _inputView.OnPauseRequested -= OnPauseRequested);

      _hudView.OnPauseClicked += OnPauseRequested;
      this.AddDisposableAction(() => _hudView.OnPauseClicked -= OnPauseRequested);
    }

    private void OnPauseRequested()
    {
      if (_isPauseOpen)
        return;

      ShowPauseAsync().Forget();
    }

    private async UniTaskVoid ShowPauseAsync()
    {
      _isPauseOpen = true;
      Time.timeScale = 0f;
      _inputView.DisableInput();

      PauseWindowResult result;
      try
      {
        result = await ExecuteAndWaitResultAsync<PauseWindowController, PauseWindowResult>(CancellationToken);
      }
      finally
      {
        Time.timeScale = 1f;
        _inputView.EnableInput();
        _isPauseOpen = false;
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
