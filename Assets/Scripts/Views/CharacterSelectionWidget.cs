using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionWidget : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button _button;
    [SerializeField] private Image _mugshotImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private Image _coverImage;
    [SerializeField] private TextMeshProUGUI _reqText;

    private Action<CharacterSelectionWidget> _onClickCallback;

    private bool _isUnlocked;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    public void FillInfo(Sprite mugshot, int requiredLevel, Action<CharacterSelectionWidget> selectionCallback)
    {
        _mugshotImage.sprite = mugshot;
        _reqText.text = $"Lvl. {requiredLevel} required";
        _onClickCallback = selectionCallback;
    }

    public void SetIsUnlocked(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
        _coverImage.gameObject.SetActive(!isUnlocked);
        _reqText.gameObject.SetActive(!isUnlocked);
    }

    public void SetSelected(bool isSelected) => _frameImage.gameObject.SetActive(isSelected);

    private void OnButtonClick()
    {
        if (_isUnlocked)
            _onClickCallback.Invoke(this);
    }

    #endregion
}
