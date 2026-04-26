using Playtika.Controllers;
using UnityEngine;

public class PlayerController : ControllerBase<LevelSceneContext>
{
    private readonly PlayerConfig _config;
    private PlayerModel _model;

    public PlayerController(IControllerFactory factory, PlayerConfig config) : base(factory)
    {
        _config = config;
    }

    protected override void OnStart()
    {
        _model = new PlayerModel();
        Args.PlayerView.Bind(_config);

        AddDisposable(new DisposableToken(() =>
        {
            Args.PlayerView.OnLaunchRequested -= OnLaunchRequested;
            Args.PlayerView.Unbind();
        }));
        Args.PlayerView.OnLaunchRequested += OnLaunchRequested;
    }

    private void OnLaunchRequested(Vector2 dragVector)
    {
        var force = dragVector * _config.LaunchForceMultiplier;
        _model.SetLaunched(force);
        Args.PlayerView.Launch(force);
    }
}
