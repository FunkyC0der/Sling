using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Common.UI;
using Sling.Infrastructure.UI;
using Sling.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.MainMenu.SelectLevel
{
  public class SelectLevelWindowController : ControllerWithResultBase<int>
  {
    private VisualElement _window;
    private int _selectedLevelIndex;
    private VisualElement _selectedRect;

    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;

    public SelectLevelWindowController(
      IControllerFactory controllerFactory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig)
      : base(controllerFactory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.SelectLevelWindowUxml, hasBackground: false);

      int result;
      try
      {
        InitWindow();
        await _popUpWindowsRootView.ShowWindow(_window, ct);
        result = await WaitForResult(ct);
        await _popUpWindowsRootView.HideWindow(_window, ct);
      }
      finally
      {
        _popUpWindowsRootView.RemoveWindow(_window);
      }

      Complete(result);
    }

    private void InitWindow()
    {
      VisualElement levelItemsContainer = _window.Q(WindowNames.LevelItemsContainer);
      levelItemsContainer.Clear();

      for (int i = 0; i < _gameConfig.Levels.Count; i++)
      {
        _gameConfig.SelectLevelLevelItemUxml.CloneTree(levelItemsContainer.contentContainer);
        VisualElement levelItem = levelItemsContainer.ElementAt(i);

        levelItem.dataSource = new LevelItemViewData() {Name = $"{i + 1}"};

        LevelType levelType = _gameConfig.Levels[i].Type;
        if(levelType == LevelType.Boss)
          levelItem.AddToClassList(WindowNames.Classes.BossLevelItem);
        else if(levelType == LevelType.SuperBoss)
          levelItem.AddToClassList(WindowNames.Classes.SuperBossLevelItem);

        int levelIndex = i;
        levelItem.AddManipulator(new Clickable(() => SelectItem(levelIndex, levelItem)));

        if (i == 0)
          SelectItem(levelIndex, levelItem);
      }
    }

    private UniTask<int> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<int>();
      
      _window.Q<Button>(WindowNames.PlayButton).clicked += () =>
        completionSource.TrySetResult(_selectedLevelIndex);

      return completionSource.Task.AttachExternalCancellation(ct);
    }
    
    private void SelectItem(int levelIndex, VisualElement levelItem)
    {
      _selectedRect?.SetCheckedPseudoState(false);

      _selectedLevelIndex = levelIndex;
      _selectedRect = levelItem.Q<VisualElement>(WindowNames.SelectedRect);
      _selectedRect.SetCheckedPseudoState(true);
    }
  }
}
