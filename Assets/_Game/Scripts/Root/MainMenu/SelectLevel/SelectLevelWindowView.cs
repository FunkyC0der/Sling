using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sling.Root.MainMenu.SelectLevel
{
  [RequireComponent(typeof(UIDocument))]
  public class SelectLevelWindowView : MonoBehaviour
  {
    private static class Names
    {
      public const string kPlayButton = "PlayButton";
    }

    public event Action<int> OnPlayClicked;

    [SerializeField] private VisualTreeAsset _levelItemTemplate;

    private Button _playButton;
    private ScrollView _scrollView;
    private UIDocument _uiDocument;

    private int _selectedLevelIndex;
    private VisualElement _selectedLevelItem;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _scrollView = _uiDocument.rootVisualElement.Q<ScrollView>();

      _playButton = _uiDocument.rootVisualElement.Q<Button>(Names.kPlayButton);
      _playButton.clicked += () => OnPlayClicked?.Invoke(_selectedLevelIndex);
    }

    public void SetLevels(IReadOnlyList<LevelItemViewData> levels)
    {
      _scrollView.Clear();

      for (int i = 0; i < levels.Count; ++i)
      {
        VisualElement levelItem = _levelItemTemplate.Instantiate()
          .Children()
          .First();

        levelItem.dataSource = levels[i];

        int levelIndex = i;
        levelItem.AddManipulator(new Clickable(() => SelectItem(levelIndex, levelItem)));

        if (i == 0)
          SelectItem(levelIndex, levelItem);

        _scrollView.Add(levelItem);
      }
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
