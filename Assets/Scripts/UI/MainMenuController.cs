using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using System.Threading;

public class MainMenuController : ControllerWithResultBase<int>
{
    private readonly MainMenuView _view;

    public MainMenuController(IControllerFactory factory, MainMenuView view) : base(factory)
    {
        _view = view;
    }

    protected override void OnStart()
    {
        AddDisposable(new DisposableToken(() => _view.OnPlayRequested -= OnPlayRequested));
        _view.OnPlayRequested += OnPlayRequested;
        _view.Show();
    }

    protected override void OnStop()
    {
        _view.Hide();
    }

    protected override UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    private void OnPlayRequested()
    {
        Complete(1); // start from level 1
    }
}
