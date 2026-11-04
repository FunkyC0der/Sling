using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Hud;
using Sling.Level.Player.States;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Player
{
  public class PlayerPauseController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerModel _model;
    private readonly HudView _hudView;
    private readonly LevelEvents _events;

    public PlayerPauseController(
      IControllerFactory controllerFactory,
      PlayerInputView inputView,
      PlayerModel model,
      HudView hudView,
      LevelEvents events)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _model = model;
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
      // No pause during the death animation or after the finish.
      if (_model.IsPaused.Value || PlayerStates.IsTerminal(_model.State))
        return;

      ShowPauseAsync().Forget();
    }

    private async UniTaskVoid ShowPauseAsync()
    {
      _model.IsPaused.Value = true;
      Time.timeScale = 0f;

      PauseWindowResult result;
      try
      {
        result = await ExecuteAndWaitResultAsync<PauseWindowController, PauseWindowResult>(CancellationToken);
      }
      finally
      {
        Time.timeScale = 1f;
        _model.IsPaused.Value = false;
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
