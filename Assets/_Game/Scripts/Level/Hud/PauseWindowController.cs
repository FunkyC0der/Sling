using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Common.UI;
using Sling.Common.UI.Windows;
using Sling.Root.Game;
using UnityEngine.UIElements;

namespace Sling.Level.Hud
{
  public class PauseWindowController : WindowControllerBase<PauseWindowResult>
  {
    private readonly GameConfig _gameConfig;

    private VisualElement _window;

    public PauseWindowController(IControllerFactory factory, GameConfig gameConfig)
      : base(factory)
    {
      _gameConfig = gameConfig;
    }

    protected override VisualTreeAsset WindowTemplate => _gameConfig.PauseWindowUxml;

    protected override void InitWindow(VisualElement window) =>
      _window = window;

    protected override UniTask<PauseWindowResult> WaitForResult(CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<PauseWindowResult>();

      _window.Q<Button>(WindowNames.RestartButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Restart);

      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(PauseWindowResult.Menu);

      return completionSource.Task.AttachExternalCancellation(cancellationToken);
    }
  }
}
