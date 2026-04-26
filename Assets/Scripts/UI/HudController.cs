using Playtika.Controllers;

public class HudController : ControllerBase<LevelSceneContext>
{
    private readonly HudView _view;

    public HudController(IControllerFactory factory, HudView view) : base(factory)
    {
        _view = view;
    }

    protected override void OnStart()
    {
        AddDisposable(new DisposableToken(() => _view.OnRestartRequested -= OnRestartRequested));
        _view.OnRestartRequested += OnRestartRequested;
        _view.Show();
    }

    protected override void OnStop()
    {
        _view.Hide();
    }

    private void OnRestartRequested()
    {
        Args.LevelEvents.RaiseRestartRequested();
    }
}
