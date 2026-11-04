using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Player.States;
using Sling.Level.Session;

namespace Sling.Level.Player
{
  /// <summary>Forwards player-scope events to the level.</summary>
  public class PlayerEventsBroadcastController : ControllerBase
  {
    private readonly PlayerModel _model;
    private readonly LevelEvents _levelEvents;

    public PlayerEventsBroadcastController(IControllerFactory controllerFactory, PlayerModel model, LevelEvents levelEvents)
      : base(controllerFactory)
    {
      _model = model;
      _levelEvents = levelEvents;
    }

    protected override void OnStart()
    {
      _model.Launched += OnLaunched;
      this.AddDisposableAction(() => _model.Launched -= OnLaunched);

      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);
    }

    private void OnLaunched() =>
      _levelEvents.OnPlayerLaunched?.Invoke();

    // Finished is terminal, so it is entered (and the level completed) exactly once.
    private void OnStateChanged(EPlayerState from, EPlayerState to)
    {
      if (to == EPlayerState.Finished)
        _levelEvents.OnLevelCompleted?.Invoke();
    }
  }
}
