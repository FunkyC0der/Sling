using System;
using System.Collections.Generic;
using System.Linq;
using Sling.Common.UI;
using UnityEngine.UIElements;

namespace Sling.Root.MainMenu.SelectLevel
{
  public class SelectLevelWindowView
  {
    public event Action<int> OnPlayClicked;

    private int _selectedLevelIndex;
    private VisualElement _selectedLevelItem;

    public SelectLevelWindowView(
      VisualElement contentRoot,
      IReadOnlyList<LevelItemViewData> levelData,
      VisualTreeAsset levelItemTemplate)
    {
      ScrollView scrollView = contentRoot.Q<ScrollView>();
      scrollView.Clear();

      for (int i = 0; i < levelData.Count; i++)
      {
        VisualElement levelItem = levelItemTemplate.Instantiate().Children().First();
        levelItem.dataSource = levelData[i];

        int levelIndex = i;
        levelItem.AddManipulator(new Clickable(() => SelectItem(levelIndex, levelItem)));

        if (i == 0)
          SelectItem(levelIndex, levelItem);

        scrollView.Add(levelItem);
      }

      contentRoot.Q<Button>(WindowNames.PlayButton).clicked += () => OnPlayClicked?.Invoke(_selectedLevelIndex);
    }

    private void SelectItem(int levelIndex, VisualElement levelItem)
    {
      _selectedLevelItem?.SetCheckedPseudoState(false);

      _selectedLevelIndex = levelIndex;
      _selectedLevelItem = levelItem;
      _selectedLevelItem.SetCheckedPseudoState(true);
    }
  }
}
