using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling;
using Sling.Common.UI;
using Sling.Common.UI.Windows;
using Sling.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.MainMenu.SelectLevel
{
  public class SelectLevelWindowController : WindowControllerBase<int>
  {
    private VisualElement _window;
    private int _selectedLevelIndex;
    private VisualElement _selectedRect;

    private readonly GameConfig _gameConfig;

    public SelectLevelWindowController(IControllerFactory controllerFactory, GameConfig gameConfig)
      : base(controllerFactory)
    {
      _gameConfig = gameConfig;
    }

    protected override VisualTreeAsset WindowTemplate => _gameConfig.SelectLevelWindowUxml;
    
    protected override void InitWindow(VisualElement window)
    {
      _window = window;
      
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

    protected override async UniTask<int> WaitForResult(CancellationToken cancellationToken)
    {
      var completionSource = new UniTaskCompletionSource<int>();
      
      _window.Q<Button>(WindowNames.PlayButton).clicked += () =>
        completionSource.TrySetResult(_selectedLevelIndex);
      
      await completionSource.Task.AttachExternalCancellation(cancellationToken);
      return _selectedLevelIndex;
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
