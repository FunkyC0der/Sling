using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Hud
{
  public class HudController : ControllerBase
  {
    private readonly HudView _hudView;
    private readonly PlayerInputView _playerInput;
    private readonly LevelEvents _events;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly UpdateEvents _updateEvents;

    public HudController(
      IControllerFactory factory,
      HudView hudView,
      PlayerInputView playerInput,
      LevelEvents events, 
      GameModel gameModel, 
      LevelModel levelModel, 
      UpdateEvents updateEvents)
      : base(factory)
    {
      _hudView = hudView;
      _playerInput = playerInput;
      _events = events;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnUpdate += Update;
      this.AddDisposableAction(() => _updateEvents.OnUpdate -= Update);
      
      _hudView.OnPauseClicked += OnPauseClicked;
      this.AddDisposableAction(() => _hudView.OnPauseClicked -= OnPauseClicked);
      
      _levelModel.PlayerDeathCount.OnValueChanged += OnPlayerDeathCountChanged;
      this.AddDisposableAction(() => _levelModel.PlayerDeathCount.OnValueChanged -= OnPlayerDeathCountChanged);

      _hudView.SetLevelIndex(_gameModel.LevelIndex);
      _hudView.SetPlayerDeathCount(_levelModel.PlayerDeathCount.Value);
    }

    private void Update() => 
      _hudView.SetLevelTime(_levelModel.ElapsedTimeInSeconds);

    private void OnPauseClicked() =>
      ShowPauseAsync().Forget();

    private void OnPlayerDeathCountChanged(int oldValue, int newValue) => 
      _hudView.SetPlayerDeathCount(newValue);

    private async UniTaskVoid ShowPauseAsync()
    {
      Time.timeScale = 0f;
      _playerInput.DisableInput();

      PauseWindowResult result;
      try
      {
        result = await ExecuteAndWaitResultAsync<PauseWindowController, PauseWindowResult>(CancellationToken);
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
