using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Common.UI;
using Sling.Common.UI.Windows;
using UnityEngine.UIElements;

namespace Sling.Level.LevelComplete
{
  public class LevelCompleteWindowController : WindowControllerBase<LevelCompleteFlowResult>
  {
    private readonly GameConfig _gameConfig;

    public LevelCompleteWindowController(IControllerFactory factory, GameConfig gameConfig)
      : base(factory)
    {
      _gameConfig = gameConfig;
    }

    private VisualElement _window;
    
    protected override VisualTreeAsset WindowTemplate => _gameConfig.LevelCompleteWindowUxml;
    
    protected override void InitWindow(VisualElement window) => 
      _window = window;

    protected override UniTask<LevelCompleteFlowResult> WaitForResult(CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<LevelCompleteFlowResult>();
      
      _window.Q<Button>(WindowNames.NextLevelButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Next);
      
      _window.Q<Button>(WindowNames.RestartButton).clicked += () => 
        completionSource.TrySetResult(LevelCompleteFlowResult.Restart);
      
      _window.Q<Button>(WindowNames.MenuButton).clicked += () =>
        completionSource.TrySetResult(LevelCompleteFlowResult.Menu);
      
      return completionSource.Task.AttachExternalCancellation(cancellationToken);
    }
  }
}
