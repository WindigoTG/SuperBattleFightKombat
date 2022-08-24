using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterSelectionUI
{
    #region Inner Classes

    [Serializable]
    private class CharacterSelectionConfig
    {
        public Character Character;
        public Sprite Mugshot;
        [Min(1)] public int RequiredLevel;
        public Sprite CharacterInfoImage;
    }

    #endregion


    #region Fields

    [SerializeField] private RectTransform _characterSelectionUiPanel;
    [SerializeField] private CharacterSelectionWidget _characterSelectionWidgetPrefab;
    [SerializeField] private RectTransform _characterSelectionWidgetParent;
    [SerializeField] private Image _characterInfoImage;
    [SerializeField] private Button _selectButton;
    [Space]
    [SerializeField] private List<CharacterSelectionConfig> _characterConfigs;

    private List<CharacterSelectionWidget> _characterWidgets = new List<CharacterSelectionWidget>();
    private Dictionary<CharacterSelectionWidget, CharacterSelectionConfig> _characterConfigsByWidget = 
                                        new Dictionary<CharacterSelectionWidget, CharacterSelectionConfig>();

    private Character _currentCharacter;

    public Action<Character> OnCharacterSelected;

    #endregion


    #region Methods

    public void Init()
    {
        _selectButton.onClick.AddListener(OnSelectedButtonClick);

        foreach (var config in _characterConfigs)
        {
            var widget = GameObject.Instantiate(_characterSelectionWidgetPrefab, _characterSelectionWidgetParent);
            widget.FillInfo(config.Mugshot, config.RequiredLevel, OnWidgetSelected);
            _characterWidgets.Add(widget);
            _characterConfigsByWidget.Add(widget, config);
        }
    }

    private void OnWidgetSelected(CharacterSelectionWidget selectedWidget)
    {
        foreach (var widget in _characterWidgets)
            widget.SetSelected(false);

        selectedWidget.SetSelected(true);

        _currentCharacter = _characterConfigsByWidget[selectedWidget].Character;
        if (_characterConfigsByWidget[selectedWidget].CharacterInfoImage != null)
        {
            _characterInfoImage.sprite = _characterConfigsByWidget[selectedWidget].CharacterInfoImage;
            _characterInfoImage.gameObject.SetActive(true);
        }
        else
            _characterInfoImage.gameObject.SetActive(false);

        _selectButton.interactable = true;
    }

    public void SetEnabled(bool isEnabled)
    {
        _characterSelectionUiPanel.gameObject.SetActive(isEnabled);

        if (isEnabled)
        {
            _selectButton.interactable = false;
            _characterInfoImage.gameObject.SetActive(false);

            foreach (var widget in _characterWidgets)
            {
                widget.SetSelected(false);
                widget.SetIsUnlocked(_characterConfigsByWidget[widget].RequiredLevel < 2);
            }
        }
    }

    private void OnSelectedButtonClick() => OnCharacterSelected?.Invoke(_currentCharacter);

    #endregion
}
