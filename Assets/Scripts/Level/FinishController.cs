using Playtika.Controllers;

public class FinishController : ControllerBase<LevelSceneContext>
{
    public FinishController(IControllerFactory factory) : base(factory) { }

    protected override void OnStart()
    {
        AddDisposable(new DisposableToken(() => Args.FinishView.OnPlayerReachedFinish -= OnFinishReached));
        Args.FinishView.OnPlayerReachedFinish += OnFinishReached;
    }

    private void OnFinishReached()
    {
        Args.LevelEvents.RaiseFinishReached();
    }
}
