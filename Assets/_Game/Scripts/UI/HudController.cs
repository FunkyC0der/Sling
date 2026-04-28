using Playtika.Controllers;
using Sling.Level;

namespace Sling.UI
{
  public class HudController : ControllerBase
  {
    private readonly HudView _view;
    private readonly LevelEvents _events;

    public HudController(IControllerFactory factory, HudView view, LevelEvents events) : base(factory)
    {
      _view = view;
      _events = events;
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
      _events.OnRestartRequested?.Invoke();
    }
  }
}