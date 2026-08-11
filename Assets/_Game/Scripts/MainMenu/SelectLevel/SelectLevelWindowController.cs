using System;
using System.Collections.Generic;
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
  public class SelectLevelWindowController : ControllerWithResultBase<LevelAddress>
  {
    // Extension point: subscribe here to drive a camera transition between world anchors in the MainMenu scene.
    public event Action<int> OnWorldChanged;

    private VisualElement _window;
    private VisualElement _levelItemsContainer;
    private int _selectedWorldIndex;
    private int _selectedLevelIndex;
    private VisualElement _selectedRect;

    private readonly PopUpWindowsRootView _popUpWindowsRootView;
    private readonly GameConfig _gameConfig;
    private readonly GameModel _gameModel;

    public SelectLevelWindowController(
      IControllerFactory controllerFactory,
      PopUpWindowsRootView popUpWindowsRootView,
      GameConfig gameConfig,
      GameModel gameModel)
      : base(controllerFactory)
    {
      _popUpWindowsRootView = popUpWindowsRootView;
      _gameConfig = gameConfig;
      _gameModel = gameModel;
    }

    protected override async UniTask OnFlowAsync(CancellationToken ct)
    {
      _window = _popUpWindowsRootView.CreateWindow(_gameConfig.SelectLevelWindowUxml, hasBackground: false);

      LevelAddress result;
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
      _levelItemsContainer = _window.Q(WindowNames.LevelItemsContainer);

      _window.Q<Button>(WindowNames.PrevWorldButton).clicked += () => SelectWorld(_selectedWorldIndex - 1);
      _window.Q<Button>(WindowNames.NextWorldButton).clicked += () => SelectWorld(_selectedWorldIndex + 1);

      SelectWorld(_gameModel.WorldIndex);
    }

    private void SelectWorld(int worldIndex)
    {
      worldIndex = Mathf.Clamp(worldIndex, 0, _gameConfig.Worlds.Count - 1);
      _selectedWorldIndex = worldIndex;

      _window.Q<Label>(WindowNames.WorldName).text = _gameConfig.Worlds[worldIndex].Name;
      _window.Q<Button>(WindowNames.PrevWorldButton).SetEnabled(worldIndex > 0);
      _window.Q<Button>(WindowNames.NextWorldButton).SetEnabled(worldIndex < _gameConfig.Worlds.Count - 1);

      BuildLevelItems(worldIndex);
      OnWorldChanged?.Invoke(worldIndex);
    }

    private void BuildLevelItems(int worldIndex)
    {
      _levelItemsContainer.Clear();
      _selectedRect = null;

      List<LevelConfig> levels = _gameConfig.Worlds[worldIndex].Levels;
      for (int i = 0; i < levels.Count; i++)
      {
        _gameConfig.SelectLevelLevelItemUxml.CloneTree(_levelItemsContainer.contentContainer);
        VisualElement levelItem = _levelItemsContainer.ElementAt(i);

        levelItem.dataSource = new LevelItemViewData() {Name = $"{i + 1}"};

        LevelType levelType = levels[i].Type;
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

    private UniTask<LevelAddress> WaitForResult(CancellationToken ct)
    {
      var completionSource = new UniTaskCompletionSource<LevelAddress>();
      
      _window.Q<Button>(WindowNames.PlayButton).clicked += () =>
        completionSource.TrySetResult(new LevelAddress(_selectedWorldIndex, _selectedLevelIndex));

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
