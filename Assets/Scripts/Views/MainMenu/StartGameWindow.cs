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
    [Space]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private TextMeshProUGUI _gamesPlayedText;
    [SerializeField] private TextMeshProUGUI _gamesWonText;
    [SerializeField] private TextMeshProUGUI _scoreText;

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
        FillStats();
    }

    private void FillStats()
    {
        _levelText.text = FirebaseManager.Instance.UserProfileHandler.UserLevel.ToString();
        _expText.text = FirebaseManager.Instance.UserProfileHandler.UserExp.ToString();
        _gamesPlayedText.text = FirebaseManager.Instance.UserProfileHandler.MatchesPlayed.ToString();
        _gamesWonText.text = FirebaseManager.Instance.UserProfileHandler.MatchesWon.ToString();
        _scoreText.text = FirebaseManager.Instance.UserProfileHandler.TotalScore.ToString();
    }

    #endregion
}
