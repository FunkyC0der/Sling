using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Infrastructure;
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
    private readonly LevelModel _levelModel;
    private readonly UpdateEvents _updateEvents;

    public HudController(
      IControllerFactory factory,
      HudView hudView,
      PopupWindowsRootView popupRootView,
      PlayerInputView playerInput,
      LevelEvents events, 
      GameModel gameModel, 
      LevelModel levelModel, 
      UpdateEvents updateEvents)
      : base(factory)
    {
      _hudView = hudView;
      _popupRootView = popupRootView;
      _playerInput = playerInput;
      _events = events;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnUpdate += Update;
      
      _hudView.OnPauseClicked += HandlePauseClicked;
      _hudView.SetLevelIndex(_gameModel.LevelIndex);
      
      _levelModel.PlayerDeathCount.OnValueChanged += (_, newValue) => _hudView.SetPlayerDeathCount(newValue);
      _hudView.SetPlayerDeathCount(_levelModel.PlayerDeathCount.Value);
    }

    protected override void OnStop()
    {
      _updateEvents.OnUpdate -= Update;
      _hudView.OnPauseClicked -= HandlePauseClicked;
    }

    private void HandlePauseClicked() =>
      ShowPauseAsync().Forget();

    private void Update() => 
      _hudView.SetLevelTime(_levelModel.ElapsedTimeInSeconds);

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
