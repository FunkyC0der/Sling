using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level;

namespace Sling.UI
{
  public class LevelResultController : ControllerWithResultBase<GameplayOutcome, NextAction>
  {
    private readonly LevelResultView _view;

    public LevelResultController(IControllerFactory factory, LevelResultView view) : base(factory)
    {
      _view = view;
    }

    protected override void OnStart()
    {
      AddDisposable(new DisposableToken(() =>
      {
        _view.OnRestartRequested -= OnRestart;
        _view.OnNextRequested -= OnNext;
        _view.OnMenuRequested -= OnMenu;
      }));
      _view.OnRestartRequested += OnRestart;
      _view.OnNextRequested += OnNext;
      _view.OnMenuRequested += OnMenu;
      _view.Show(Args);
    }

    protected override void OnStop()
    {
      _view.Hide();
    }

    protected override UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      return UniTask.CompletedTask;
    }

    private void OnRestart()
    {
      Complete(NextAction.Restart);
    }

    private void OnNext()
    {
      Complete(NextAction.Next);
    }

    private void OnMenu()
    {
      Complete(NextAction.Menu);
    }
  }
}