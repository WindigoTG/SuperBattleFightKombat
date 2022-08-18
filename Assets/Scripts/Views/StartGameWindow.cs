using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class StartGameWindow
{
    #region Fields

    [SerializeField] private RectTransform _windowPanel;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _changeNameButton;
    [SerializeField] private Button _switchAccountButton;
    [SerializeField] private TextMeshProUGUI _welcomeText;

    #endregion


    #region Properties

    public RectTransform WindowPanel => _windowPanel;
    public Button StartGameButton => _startGameButton;
    public Button ChangeNameButton => _changeNameButton;
    public Button SwitchAccountButton => _switchAccountButton;

    #endregion


    #region Methods

    public void FillWelcomeText()
    {
        var name = FirebaseManager.Instance.UserProfileHandler.UserName;
        _welcomeText.text = $"Welcome, {name}!";
    }

    #endregion
}
