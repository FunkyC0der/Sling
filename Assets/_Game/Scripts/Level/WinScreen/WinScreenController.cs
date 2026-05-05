using Playtika.Controllers;
using UnityEditorInternal;

namespace Sling.Level.WinScreen
{
  public class WinScreenController : ControllerWithResultBase<WinScreenResult>
  {
    private readonly WinScreenView _view;

    public WinScreenController(IControllerFactory factory, WinScreenView view)
      : base(factory)
    {
      _view = view;
    }

    protected override void OnStart()
    {
      _view.OnRestartClicked += OnRestartClicked;
      AddDisposable(new DisposableToken(() => _view.OnRestartClicked -= OnRestartClicked));
      
      _view.Show();
    }

    private void OnRestartClicked() => 
      Complete(WinScreenResult.Restart);
  }
}
