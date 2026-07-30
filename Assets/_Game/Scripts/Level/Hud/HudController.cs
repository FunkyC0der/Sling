using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Session;

namespace Sling.Level.Hud
{
  public class HudController : ControllerBase
  {
    private readonly HudView _hudView;
    private readonly GameModel _gameModel;
    private readonly LevelModel _levelModel;
    private readonly UpdateEvents _updateEvents;

    public HudController(
      IControllerFactory factory,
      HudView hudView,
      GameModel gameModel, 
      LevelModel levelModel, 
      UpdateEvents updateEvents)
      : base(factory)
    {
      _hudView = hudView;
      _gameModel = gameModel;
      _levelModel = levelModel;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _updateEvents.OnUpdate += Update;
      this.AddDisposableAction(() => _updateEvents.OnUpdate -= Update);

      _levelModel.PlayerDeathCount.OnValueChanged += OnPlayerDeathCountChanged;
      this.AddDisposableAction(() => _levelModel.PlayerDeathCount.OnValueChanged -= OnPlayerDeathCountChanged);

      _levelModel.PlayerLaunchCount.OnValueChanged += OnPlayerLaunchCountChanged;
      this.AddDisposableAction(() => _levelModel.PlayerLaunchCount.OnValueChanged -= OnPlayerLaunchCountChanged);

      _hudView.SetLevelIndex(_gameModel.LevelIndex);
      _hudView.SetPlayerDeathCount(_levelModel.PlayerDeathCount.Value);
      _hudView.SetPlayerLaunchCount(_levelModel.PlayerLaunchCount.Value);
    }

    private void Update() => 
      _hudView.SetLevelTime(_levelModel.ElapsedTimeInSeconds);

    private void OnPlayerDeathCountChanged(int oldValue, int newValue) => 
      _hudView.SetPlayerDeathCount(newValue);

    private void OnPlayerLaunchCountChanged(int oldValue, int newValue) =>
      _hudView.SetPlayerLaunchCount(newValue);
  }
}
