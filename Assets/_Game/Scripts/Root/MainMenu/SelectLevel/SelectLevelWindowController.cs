using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI.Windows;
using Sling.Root.Game;
using UnityEngine.UIElements;

namespace Sling.Root.MainMenu.SelectLevel
{
  public class SelectLevelWindowController : WindowControllerBase<SelectLevelWindowView, int>
  {
    private readonly GameConfig _gameConfig;

    public SelectLevelWindowController(IControllerFactory controllerFactory, GameConfig gameConfig)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
    }

    protected override VisualTreeAsset Uxml => _gameConfig.SelectLevelWindowUxml;

    protected override SelectLevelWindowView CreateView(VisualElement contentRoot)
    {
      IReadOnlyList<LevelItemViewData> levelNames = _gameConfig.LevelScenes
        .Select(scene => new LevelItemViewData() {Name = scene.SceneName})
        .ToArray();

      return new SelectLevelWindowView(contentRoot, levelNames, _gameConfig.SelectLevelLevelItemUxml);
    }

    protected override async UniTask<int> AwaitResultAsync(
      SelectLevelWindowView view,
      CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<int>();
      view.OnPlayClicked += index => completionSource.TrySetResult(index);
      return await completionSource.Task.AttachExternalCancellation(cancellationToken);
    }
  }
}
