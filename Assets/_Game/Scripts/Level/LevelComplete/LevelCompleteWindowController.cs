using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Root.Game;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowController : WindowControllerBase<LevelCompleteWindowView, LevelCompleteFlowResult>
  {
    private readonly GameConfig _gameConfig;

    public LevelCompleteWindowController(IControllerFactory factory, GameConfig gameConfig)
      : base(factory)
    {
      _gameConfig = gameConfig;
    }

    protected override VisualTreeAsset Uxml => _gameConfig.LevelCompleteWindowUxml;
    protected override LevelCompleteWindowView CreateView(VisualElement contentRoot) => new(contentRoot);

    protected override async UniTask<LevelCompleteFlowResult> AwaitResultAsync(
      LevelCompleteWindowView view,
      CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<LevelCompleteFlowResult>();
      view.OnNextLevelClicked += () => completionSource.TrySetResult(LevelCompleteFlowResult.Next);
      view.OnRestartClicked   += () => completionSource.TrySetResult(LevelCompleteFlowResult.Restart);
      view.OnMenuClicked      += () => completionSource.TrySetResult(LevelCompleteFlowResult.Menu);
      return await completionSource.Task.AttachExternalCancellation(cancellationToken);
    }
  }
}
