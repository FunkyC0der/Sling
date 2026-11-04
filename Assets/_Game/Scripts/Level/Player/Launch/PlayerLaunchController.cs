using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Player.States;
using UnityEngine;

namespace Sling.Level.Player.Launch
{
  public class PlayerLaunchController : ControllerBase
  {
    private readonly PlayerInputView _inputView;
    private readonly PlayerModel _model;
    private readonly PlayerLaunchView _launchView;
    private readonly PlayerConfig _config;

    private CancellationTokenSource _aimCts;

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
      this.AddDisposableAction(() => _inputView.OnPreLaunchStart -= OnPreLaunchStart);

      _model.IsTouchingSurface.OnValueChanged += OnTouchingSurfaceChanged;
      this.AddDisposableAction(() => _model.IsTouchingSurface.OnValueChanged -= OnTouchingSurfaceChanged);

      _model.StateChanged += OnStateChanged;
      this.AddDisposableAction(() => _model.StateChanged -= OnStateChanged);

      this.AddDisposableAction(CancelAim);
    }

    private void OnPreLaunchStart()
    {
      if (!_model.CanLaunch || _aimCts != null)
        return;

      LaunchFlowAsync().Forget();
    }

    private async UniTask LaunchFlowAsync()
    {
      _aimCts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
      CancellationToken token = _aimCts.Token;

      Vector2 launchVelocity;
      try
      {
        launchVelocity = await ExecuteAndWaitResultAsync<PlayerPreLaunchFlowController, Vector2>(token);
      }
      catch (System.OperationCanceledException)
      {
        return;
      }
      finally
      {
        DisposeAimCts();
      }

      // State may have become terminal while aiming (cancellation is the primary guard; this is the backup).
      if (launchVelocity.sqrMagnitude <= 0 || !_model.CanLaunch)
        return;

      _model.RemainingLaunches = Mathf.Max(0, _model.RemainingLaunches - 1);

      _launchView.Launch(launchVelocity);
      _model.RaiseLaunched();
    }

    private void OnStateChanged(EPlayerState from, EPlayerState to)
    {
      if (!PlayerStates.AllowsLaunch(to))
        CancelAim();
    }

    private void OnTouchingSurfaceChanged(bool oldValue, bool newValue)
    {
      if (newValue)
        ResetRemainingLaunches();
    }

    private void ResetRemainingLaunches() =>
      _model.RemainingLaunches = 1 + _config.MaxAirLaunches;

    private void CancelAim() =>
      _aimCts?.Cancel();

    private void DisposeAimCts()
    {
      _aimCts?.Dispose();
      _aimCts = null;
    }
  }
}
