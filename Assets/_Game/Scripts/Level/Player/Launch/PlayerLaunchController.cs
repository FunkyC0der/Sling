using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using UnityEngine;

namespace Sling.Level.Player.Launch
{
  public class PlayerLaunchController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerModel _model;
    private readonly PlayerLaunchView _launchView;
    private readonly PlayerConfig _config;

    private int _remainingLaunches;

    public PlayerLaunchController(IControllerFactory controllerFactory,
      PlayerInputView inputView,
      PlayerModel model, 
      PlayerLaunchView launchView,
      PlayerConfig config)
      : base(controllerFactory)
    {
      _inputView = inputView;
      _model = model;
      _launchView = launchView;
      _config = config;
    }

    protected override void OnStart()
    {
      ResetRemainingLaunches();
      
      _inputView.OnPreLaunchStart += OnPreLaunchStart;
      _model.IsInAir.OnValueChanged += OnIsInAirChanged;
    }

    protected override void OnStop()
    {
      _model.IsInAir.OnValueChanged -= OnIsInAirChanged;
      _inputView.OnPreLaunchStart -= OnPreLaunchStart;
    }

    private void OnPreLaunchStart(Vector2 worldPos)
    {
      if (_model.IsInAir.Value && _remainingLaunches <= 0)
        return;

      LaunchFlowAsync(worldPos).Forget();
    }

    private async UniTask LaunchFlowAsync(Vector2 worldPos)
    {
      _model.OnPreLaunch?.Invoke();
      
      Vector2 launchVelocity =
        await ExecuteAndWaitResultAsync<PlayerPreLaunchFlowController, Vector2, Vector2>(worldPos, CancellationToken);

      if (launchVelocity.sqrMagnitude > 0)
      {
        _remainingLaunches = Mathf.Max(0, _remainingLaunches - 1);
        
        _launchView.Launch(launchVelocity);
        _model.OnLaunched?.Invoke();
      }      
    }

    private void OnIsInAirChanged(bool oldValue, bool newValue)
    {
      if (!newValue) 
        ResetRemainingLaunches();
    }

    private void ResetRemainingLaunches() => 
      _remainingLaunches = 1 + _config.MaxAirLaunches;
  }
}
