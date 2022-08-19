using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class ChangeNameWindow
{
    #region Fields

    [SerializeField] private RectTransform _windowPanel;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private Action _onCloseCallback;

    #endregion


    #region Properties

    public void AddListeners()
    {
        _confirmButton.onClick.AddListener(OnConfirmButtonClick);
        _confirmButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(References.BUTTON_SOUND));

        _cancelButton.onClick.AddListener(OnCancelButtonClick);
        _cancelButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(References.BUTTON_SOUND));
    }

    public void RemoveListeners()
    {
        _confirmButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }

    public void SetWindowActive(bool isActive)
    {
        if (isActive)
            _nameInput.text = FirebaseManager.Instance.UserProfileHandler.UserName;
        else
            _onCloseCallback?.Invoke();

        _windowPanel.gameObject.SetActive(isActive);
    }

    public void SetOnCloseCallback(Action callback)
    {
        _onCloseCallback = callback;
    }

    private void OnCancelButtonClick()
    {
        SetWindowActive(false);
    }

    private void OnConfirmButtonClick()
    {
        if (!string.IsNullOrWhiteSpace(_nameInput.text))
            FirebaseManager.Instance.UserProfileHandler.SetUserName(_nameInput.text);

        SetWindowActive(false);
    }

    #endregion
}
